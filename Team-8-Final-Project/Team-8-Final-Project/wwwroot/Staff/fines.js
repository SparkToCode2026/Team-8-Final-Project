// ============================================================
// fines.js — powers staff/fines.html
// ============================================================

document.addEventListener("DOMContentLoaded", () => {
  loadFines();
  loadTotalUnpaid();
  setupAddFineForm();
});

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

    // FineController takes the raw entity, not a DTO - loanId, fineAmount,
    // fineIssueDate are all it needs; status gets forced to Unpaid server-side
    const fine = {
      loanId: Number(document.getElementById("fineLoanId").value),
      fineAmount: Number(document.getElementById("fineAmount").value),
      fineIssueDate: document.getElementById("fineIssueDate").value
    };

    try {
      await createFine(fine);
      event.target.reset();
      loadFines();
      loadTotalUnpaid();
    } catch (err) {
      alert("Could not issue fine: " + err.message);
    }
  });
}
