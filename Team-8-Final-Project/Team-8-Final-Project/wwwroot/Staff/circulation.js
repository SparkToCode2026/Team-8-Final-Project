// ============================================================
// circulation.js — powers staff/circulation.html
//
// Two tabs: Loans and Reservations. Each row has a status dropdown + an
// "Update" button (calls UpdateLoanStatus / UpdateReservationStatus)
// plus a Delete button.
// ============================================================

document.addEventListener("DOMContentLoaded", () => {
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
  // GetAllLoans includes BookCopy and User
  const borrower = loan.user ? `${loan.user.firstName} ${loan.user.lastName}` : "Unknown";
  const statuses = ["Active", "Overdue", "Returned"];

  return `
    <div class="card mb-2" data-loan-id="${loan.loanId}">
      <div class="card-body d-flex justify-content-between align-items-center flex-wrap gap-2">
        <div>
          <strong>Copy #${loan.bookCopyId}</strong>
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

    const loan = {
      bookCopyId: Number(document.getElementById("loanBookCopyId").value),
      userID: Number(document.getElementById("loanUserId").value),
      loanDueDate: document.getElementById("loanDueDate").value
    };

    try {
      await createLoan(loan);
      event.target.reset();
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
  // GetAllReservation doesn't Include anything, so this only has raw ids
  const statuses = ["Active", "Cancelled", "Completed"];

  return `
    <div class="card mb-2" data-reservation-id="${reservation.reservationId}">
      <div class="card-body d-flex justify-content-between align-items-center flex-wrap gap-2">
        <div>
          <strong>Book #${reservation.bookId}</strong>
          <span class="text-muted"> - User #${reservation.userId}, reserved ${new Date(reservation.reservationDate).toLocaleDateString()}</span>
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

    const reservation = {
      bookId: Number(document.getElementById("reservationBookId").value),
      userId: Number(document.getElementById("reservationUserId").value),
      reservationDate: new Date().toISOString()
    };

    try {
      await createReservation(reservation);
      event.target.reset();
      loadReservations();
    } catch (err) {
      alert("Could not add reservation: " + err.message);
    }
  });
}
