// ============================================================
// circulation.js — powers staff/circulation.html
//
// Two tabs: Loans and Reservations. Each row has a status dropdown + an
// "Update" button (calls UpdateLoanStatus / UpdateReservationStatus)
// plus a Delete button. The "check out"/"add reservation" forms use the
// shared search pickers (pickers.js) instead of raw numeric ID fields, so
// staff search by book/copy/member name instead of memorizing IDs.
// ============================================================

let bookCopiesForPicker = [];
let usersForPicker = [];
let booksForPicker = [];

let loanCopyPicker, loanUserPicker, reservationBookPicker, reservationUserPicker;

document.addEventListener("DOMContentLoaded", async () => {
  // Loaded up front so the pickers (and the readable names in the lists
  // below) have something to search/match against as soon as the page is
  // usable, rather than firing a fresh request per keystroke.
  try {
    [bookCopiesForPicker, usersForPicker, booksForPicker] = await Promise.all([
      getAllBookCopies(),
      getAllUsers(),
      getBooks()
    ]);
  } catch (err) {
    console.error("Could not load reference data for the search pickers:", err);
  }

  loanCopyPicker = createSearchPicker({
    containerId: "loanBookCopyPicker",
    items: () => bookCopiesForPicker,
    getId: c => c.bookCopyId,
    getLabel: c => `${c.barcode} - ${c.book?.bookTitle ?? "Unknown book"} (${c.availabilityStatus})`,
    placeholder: "Search copy by barcode or book title..."
  });

  loanUserPicker = createSearchPicker({
    containerId: "loanUserPicker",
    items: () => usersForPicker,
    getId: u => extractUserRecordId(u),
    getLabel: u => `${u.firstName} ${u.lastName} (ID ${extractUserRecordId(u)}) - ${u.userEmail}`,
    placeholder: "Search member by name..."
  });

  reservationBookPicker = createSearchPicker({
    containerId: "reservationBookPicker",
    items: () => booksForPicker,
    getId: b => b.bookId,
    getLabel: b => `${b.bookTitle} (Edition ${b.bookEdition ?? "N/A"})`,
    placeholder: "Search book by title..."
  });

  reservationUserPicker = createSearchPicker({
    containerId: "reservationUserPicker",
    items: () => usersForPicker,
    getId: u => extractUserRecordId(u),
    getLabel: u => `${u.firstName} ${u.lastName} (ID ${extractUserRecordId(u)}) - ${u.userEmail}`,
    placeholder: "Search member by name..."
  });

  loadLoans();
  loadReservations();
  setupAddLoanForm();
  setupAddReservationForm();
});

// ---- Loans ----

async function loadLoans() {
  const container = document.getElementById("loansContainer");
  try {
    const loans = await getAllLoans();
    container.innerHTML = loans.length ? loans.map(renderLoanRow).join("") : '<p class="text-muted">No loans yet.</p>';
    attachLoanHandlers();
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">${err.message}</div>`;
  }
}

function renderLoanRow(loan) {
  // GetAllLoans includes BookCopy (and Book off of that) and User
  const borrower = loan.user ? `${loan.user.firstName} ${loan.user.lastName}` : "Unknown";
  const title = loan.bookCopy?.book?.bookTitle;
  const statuses = ["Active", "Overdue", "Returned"];

  return `
    <div class="card mb-2" data-loan-id="${loan.loanId}">
      <div class="card-body d-flex justify-content-between align-items-center flex-wrap gap-2">
        <div>
          <strong>${title ? title : `Copy #${loan.bookCopyId}`}</strong>
          <span class="text-muted"> - ${borrower}, due ${new Date(loan.loanDueDate).toLocaleDateString()}</span>
        </div>
        <div class="d-flex gap-2">
          <select class="form-select form-select-sm loan-status-select">
            ${statuses.map(s => `<option value="${s}" ${s === loan.loanStatus ? "selected" : ""}>${s}</option>`).join("")}
          </select>
          <button class="btn btn-sm btn-outline-primary update-loan-btn">Update</button>
          <button class="btn btn-sm btn-outline-danger delete-loan-btn">Delete</button>
        </div>
      </div>
    </div>
  `;
}

function attachLoanHandlers() {
  document.querySelectorAll(".update-loan-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      const card = btn.closest("[data-loan-id]");
      const newStatus = card.querySelector(".loan-status-select").value;
      try { await updateLoanStatus(card.dataset.loanId, newStatus); loadLoans(); }
      catch (err) { alert("Could not update loan: " + err.message); }
    });
  });

  document.querySelectorAll(".delete-loan-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      const card = btn.closest("[data-loan-id]");
      if (!confirm("Delete this loan record?")) return;
      try { await deleteLoan(card.dataset.loanId); loadLoans(); }
      catch (err) { alert("Could not delete loan: " + err.message); }
    });
  });
}

function setupAddLoanForm() {
  document.getElementById("addLoanForm").addEventListener("submit", async (event) => {
    event.preventDefault();

    const bookCopyId = loanCopyPicker.getSelectedId();
    const userID = loanUserPicker.getSelectedId();

    if (!bookCopyId) { alert("Search for the book copy and select it from the list."); return; }
    if (!userID) { alert("Search for the member and select them from the list."); return; }

    const loan = {
      bookCopyId: Number(bookCopyId),
      userID: Number(userID),
      loanDueDate: document.getElementById("loanDueDate").value
    };

    try {
      await createLoan(loan);
      event.target.reset();
      loanCopyPicker.reset();
      loanUserPicker.reset();
      loadLoans();
    } catch (err) {
      alert("Could not check out book: " + err.message);
    }
  });
}

// ---- Reservations ----

async function loadReservations() {
  const container = document.getElementById("reservationsContainer");
  try {
    const reservations = await getAllReservations();
    container.innerHTML = reservations.length ? reservations.map(renderReservationRow).join("") : '<p class="text-muted">No reservations yet.</p>';
    attachReservationHandlers();
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">${err.message}</div>`;
  }
}

function renderReservationRow(reservation) {
  // GetAllReservation doesn't Include anything, so this only has raw ids -
  // matched up here against the same booksForPicker/usersForPicker lists the
  // search pickers use, so the list reads names/titles instead of bare ids.
  const book = booksForPicker.find(b => String(b.bookId) === String(reservation.bookId));
  const user = usersForPicker.find(u => String(extractUserRecordId(u)) === String(reservation.userId));
  const statuses = ["Active", "Cancelled", "Completed"];

  return `
    <div class="card mb-2" data-reservation-id="${reservation.reservationId}">
      <div class="card-body d-flex justify-content-between align-items-center flex-wrap gap-2">
        <div>
          <strong>${book ? book.bookTitle : `Book #${reservation.bookId}`}</strong>
          <span class="text-muted"> - ${user ? `${user.firstName} ${user.lastName}` : `User #${reservation.userId}`}, reserved ${new Date(reservation.reservationDate).toLocaleDateString()}</span>
        </div>
        <div class="d-flex gap-2">
          <select class="form-select form-select-sm reservation-status-select">
            ${statuses.map(s => `<option value="${s}" ${s === reservation.status ? "selected" : ""}>${s}</option>`).join("")}
          </select>
          <button class="btn btn-sm btn-outline-primary update-reservation-btn">Update</button>
          <button class="btn btn-sm btn-outline-danger delete-reservation-btn">Delete</button>
        </div>
      </div>
    </div>
  `;
}

function attachReservationHandlers() {
  document.querySelectorAll(".update-reservation-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      const card = btn.closest("[data-reservation-id]");
      const newStatus = card.querySelector(".reservation-status-select").value;
      try { await updateReservationStatus(card.dataset.reservationId, newStatus); loadReservations(); }
      catch (err) { alert("Could not update reservation: " + err.message); }
    });
  });

  document.querySelectorAll(".delete-reservation-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      const card = btn.closest("[data-reservation-id]");
      if (!confirm("Delete this reservation?")) return;
      try { await deleteReservation(card.dataset.reservationId); loadReservations(); }
      catch (err) { alert("Could not delete reservation: " + err.message); }
    });
  });
}

function setupAddReservationForm() {
  document.getElementById("addReservationForm").addEventListener("submit", async (event) => {
    event.preventDefault();

    const bookId = reservationBookPicker.getSelectedId();
    const userId = reservationUserPicker.getSelectedId();

    if (!bookId) { alert("Search for the book and select it from the list."); return; }
    if (!userId) { alert("Search for the member and select them from the list."); return; }

    const reservation = {
      bookId: Number(bookId),
      userId: Number(userId),
      reservationDate: new Date().toISOString()
    };

    try {
      await createReservation(reservation);
      event.target.reset();
      reservationBookPicker.reset();
      reservationUserPicker.reset();
      loadReservations();
    } catch (err) {
      alert("Could not add reservation: " + err.message);
    }
  });
}
