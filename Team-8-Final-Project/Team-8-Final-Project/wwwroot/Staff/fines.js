// ============================================================
// fines.js — powers staff/fines.html
// ============================================================

let usersForPicker = [];
let loansForPicker = [];
let fineUserPicker;
let selectedLoanId = null;

document.addEventListener("DOMContentLoaded", async () => {
  try {
    [usersForPicker, loansForPicker] = await Promise.all([getAllUsers(), getAllLoans()]);
  } catch (err) {
    console.error("Could not load reference data for the member/loan picker:", err);
  }

  fineUserPicker = createSearchPicker({
    containerId: "fineUserPicker",
    items: () => usersForPicker,
    getId: u => extractUserRecordId(u),
    getLabel: u => `${u.firstName} ${u.lastName} (ID ${extractUserRecordId(u)}) - ${u.userEmail}`,
    placeholder: "Search member by name...",
    onSelect: (user) => showLoansForUser(user)
  });

  loadFines();
  loadTotalUnpaid();
  setupAddFineForm();
});

// Shows every loan belonging to the picked member as its own clickable
// card - a member can have more than one loan out at once, and this is also
// how two members who happen to share a name get told apart (each loan
// shown here is unambiguously tied to the one user id just picked above).
function showLoansForUser(user) {
  const container = document.getElementById("fineUserLoans");
  const userId = extractUserRecordId(user);
  const matchingLoans = loansForPicker.filter(loan => String(extractUserRecordId(loan.user)) === String(userId));

  selectedLoanId = null;
  document.getElementById("fineLoanId").value = "";

  if (matchingLoans.length === 0) {
    container.innerHTML = `<p class="text-muted small mb-0">${user.firstName} ${user.lastName} has no loans on record.</p>`;
    return;
  }

  container.innerHTML = matchingLoans.map(loan => {
    const title = loan.bookCopy?.book?.bookTitle ?? `Copy #${loan.bookCopyId}`;
    return `
      <button type="button" class="list-group-item list-group-item-action fine-loan-option" data-loan-id="${loan.loanId}">
        Loan #${loan.loanId} - ${title}, due ${new Date(loan.loanDueDate).toLocaleDateString()} (${loan.loanStatus})
      </button>
    `;
  }).join("");

  container.querySelectorAll(".fine-loan-option").forEach(btn => {
    btn.addEventListener("click", () => {
      container.querySelectorAll(".fine-loan-option").forEach(b => b.classList.remove("active"));
      btn.classList.add("active");
      selectedLoanId = btn.dataset.loanId;
      document.getElementById("fineLoanId").value = selectedLoanId;
    });
  });
}

async function loadFines() {
  const container = document.getElementById("finesContainer");
  try {
    const fines = await getAllFines();
    container.innerHTML = fines.length ? fines.map(renderFineRow).join("") : '<p class="text-muted">No fines yet.</p>';
    attachFineHandlers();
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">${err.message}</div>`;
  }
}

async function loadTotalUnpaid() {
  const container = document.getElementById("totalUnpaid");
  try {
    const result = await getTotalUnpaidFines();
    container.innerHTML = `<strong>Total unpaid across all members:</strong> $${result.totalUnpaid.toFixed(2)}`;
  } catch (err) {
    container.innerHTML = "";
  }
}

function renderFineRow(fine) {
  // GetAllFines includes Loan, then BookCopy off of that
  const bookCopyId = fine.loan?.bookCopyId ?? "?";
  const statuses = ["Paid", "Unpaid", "Dismissed"];

  return `
    <div class="card mb-2" data-fine-id="${fine.fineId}">
      <div class="card-body d-flex justify-content-between align-items-center flex-wrap gap-2">
        <div>
          <strong>$${fine.fineAmount.toFixed(2)}</strong>
          <span class="text-muted"> - Loan #${fine.loanId} (Copy #${bookCopyId}), issued ${new Date(fine.fineIssueDate).toLocaleDateString()}</span>
        </div>
        <div class="d-flex gap-2">
          <select class="form-select form-select-sm fine-status-select">
            ${statuses.map(s => `<option value="${s}" ${s === fine.status ? "selected" : ""}>${s}</option>`).join("")}
          </select>
          <button class="btn btn-sm btn-outline-primary update-fine-btn">Update</button>
          <button class="btn btn-sm btn-outline-danger delete-fine-btn">Delete</button>
        </div>
      </div>
    </div>
  `;
}

function attachFineHandlers() {
  document.querySelectorAll(".update-fine-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      const card = btn.closest("[data-fine-id]");
      const newStatus = card.querySelector(".fine-status-select").value;
      try {
        await updateFineStatus(card.dataset.fineId, newStatus);
        loadFines();
        loadTotalUnpaid();
      } catch (err) {
        alert("Could not update fine: " + err.message);
      }
    });
  });

  document.querySelectorAll(".delete-fine-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      const card = btn.closest("[data-fine-id]");
      if (!confirm("Delete this fine?")) return;
      try {
        await deleteFine(card.dataset.fineId);
        loadFines();
        loadTotalUnpaid();
      } catch (err) {
        alert("Could not delete fine: " + err.message);
      }
    });
  });
}

function setupAddFineForm() {
  document.getElementById("addFineForm").addEventListener("submit", async (event) => {
    event.preventDefault();

    const loanIdValue = document.getElementById("fineLoanId").value;
    if (!loanIdValue) {
      alert("Search for the member, then click one of their loans below before issuing a fine.");
      return;
    }

    // FineController takes the raw entity, not a DTO - loanId, fineAmount,
    // fineIssueDate are all it needs; status gets forced to Unpaid server-side
    const fine = {
      loanId: Number(loanIdValue),
      fineAmount: Number(document.getElementById("fineAmount").value),
      fineIssueDate: document.getElementById("fineIssueDate").value
    };

    try {
      await createFine(fine);
      event.target.reset();
      fineUserPicker.reset();
      document.getElementById("fineUserLoans").innerHTML = '<p class="text-muted small mb-0">Search for a member above to see their loans.</p>';
      loadFines();
      loadTotalUnpaid();
    } catch (err) {
      alert("Could not issue fine: " + err.message);
    }
  });
}
