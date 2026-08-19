// ============================================================
// my-fines.js — powers my-fines.html
//
// GetTotalUnpaidFines exists on the backend but is Librarian/Admin-only,
// so the "total owed" figure here is just added up client-side from
// whatever GetMyFines (member-accessible) returns instead.
// ============================================================

document.addEventListener("DOMContentLoaded", loadMyFines);

async function loadMyFines() {
  const container = document.getElementById("finesContainer");
  const totalContainer = document.getElementById("totalOwed");

  try {
    // GetMyFines includes the Loan but not the BookCopy/Book inside it, so
    // this fetches every book copy separately and matches them up below.
    const [fines, bookCopies] = await Promise.all([
      getMyFines(),
      getAllBookCopies()
    ]);

    if (fines.length === 0) {
      container.innerHTML = '<p class="text-muted">You have no fines. Nice work!</p>';
      totalContainer.innerHTML = "";
      return;
    }

    // bookCopyId -> book copy (with its book attached), for quick lookup below
    const copyLookup = {};
    bookCopies.forEach(copy => { copyLookup[copy.bookCopyId] = copy; });

    const totalUnpaid = fines
      .filter(f => f.status === "Unpaid")
      .reduce((sum, f) => sum + f.fineAmount, 0);

    totalContainer.innerHTML = `<strong>Total unpaid:</strong> $${totalUnpaid.toFixed(2)}`;
    container.innerHTML = fines.map(f => renderFineCard(f, copyLookup[f.loan?.bookCopyId])).join("");
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">Could not load fines: ${err.message}</div>`;
  }
}

function renderFineCard(fine, bookCopy) {
  const title = bookCopy?.book?.bookTitle ?? "Unknown book";

  // Maps each FinePaymentStatus enum value to a Bootstrap badge color
  const statusClass = {
    Paid: "bg-success",
    Unpaid: "bg-danger",
    Dismissed: "bg-secondary"
  }[fine.status] ?? "bg-secondary";

  return `
    <div class="card mb-3">
      <div class="card-body d-flex justify-content-between align-items-center">
        <div>
          <h5 class="card-title mb-1">${title}</h5>
          <p class="card-text text-muted mb-0">
            Issued: ${new Date(fine.fineIssueDate).toLocaleDateString()}
            &nbsp;|&nbsp;
            $${fine.fineAmount.toFixed(2)}
          </p>
        </div>
        <span class="badge ${statusClass}">${fine.status}</span>
      </div>
    </div>
  `;
}
