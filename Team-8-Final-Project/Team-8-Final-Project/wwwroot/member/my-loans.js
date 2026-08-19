// ============================================================
// my-loans.js — powers my-loans.html
//
// This page is read-only. UpdateLoanStatus (the endpoint that marks a
// loan Returned) is Librarian/Admin-only server-side, so a member can't
// act on their own loans here - only look at them. If you later add a
// "return" flow, that's the endpoint it would need to be relaxed on.
// ============================================================

document.addEventListener("DOMContentLoaded", loadMyLoans);

async function loadMyLoans() {
  const container = document.getElementById("loansContainer");
  const userId = getUserId();

  try {
    // GetLoansByUser doesn't include the book copy or book, so it fetches
    // every book copy separately and matches them up client-side below.
    const [loans, bookCopies] = await Promise.all([
      getLoansByUser(userId),
      getAllBookCopies()
    ]);

    if (loans.length === 0) {
      container.innerHTML = '<p class="text-muted">You have no loans yet.</p>';
      return;
    }

    // bookCopyId -> book copy (with its book attached), for quick lookup below
    const copyLookup = {};
    bookCopies.forEach(copy => { copyLookup[copy.bookCopyId] = copy; });

    container.innerHTML = loans.map(loan => renderLoanCard(loan, copyLookup[loan.bookCopyId])).join("");
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">Could not load loans: ${err.message}</div>`;
  }
}

function renderLoanCard(loan, bookCopy) {
  const title = bookCopy?.book?.bookTitle ?? "Unknown book";

  // Maps each LoanStatus enum value to a Bootstrap badge color
  const statusClass = {
    Active: "bg-primary",
    Overdue: "bg-danger",
    Returned: "bg-success"
  }[loan.loanStatus] ?? "bg-secondary";

  return `
    <div class="card mb-3">
      <div class="card-body d-flex justify-content-between align-items-center">
        <div>
          <h5 class="card-title mb-1">${title}</h5>
          <p class="card-text text-muted mb-0">
            Borrowed: ${new Date(loan.loanStartDate).toLocaleDateString()}
            &nbsp;|&nbsp;
            Due: ${new Date(loan.loanDueDate).toLocaleDateString()}
          </p>
        </div>
        <span class="badge ${statusClass}">${loan.loanStatus}</span>
      </div>
    </div>
  `;
}
