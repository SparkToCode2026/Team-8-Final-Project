// ============================================================
// fines.js — powers staff/fines.html
//
// The fines table is enriched client-side by cross-referencing each fine's
// loanId against the already-loaded loansForPicker list (from GetAllLoans,
// which reliably includes BookCopy -> Book -> User) instead of trusting
// fine.loan directly - GetAllFines' own Loan include appears to be missing
// or broken server-side (book copy showed as "Copy #?" even though the
// exact same loan displays correctly everywhere else). Ask about
// FineController.GetAllFines / Fine.cs if you want the root cause fixed
// server-side too - this workaround doesn't depend on that being fixed.
// ============================================================

const FINES_PAGE_SIZE = 20;

let usersForPicker = [];
let loansForPicker = [];
let allFines = [];
let fineUserPicker;
let selectedLoanId = null;

let finesFilterUserId = null; // null = show every member's fines
let finesSortOrder = "newest";
let finesVisibleCount = FINES_PAGE_SIZE;

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
    onSelect: (user) => {
      showLoansForUser(user);
      // Searching a member here does double duty: it also filters the fines
      // table below to that member's fines, so there's one search instead
      // of two for "issue this person a fine AND see what they already owe."
      finesFilterUserId = extractUserRecordId(user);
      finesVisibleCount = FINES_PAGE_SIZE;
      renderFinesTable();
    }
  });

  document.getElementById("clearFineFilterBtn").addEventListener("click", () => {
    finesFilterUserId = null;
    finesVisibleCount = FINES_PAGE_SIZE;
    renderFinesTable();
  });

  document.getElementById("fineSortOrder").addEventListener("change", (event) => {
    finesSortOrder = event.target.value;
    renderFinesTable();
  });

  document.getElementById("loadMoreFinesBtn").addEventListener("click", () => {
    finesVisibleCount += FINES_PAGE_SIZE;
    renderFinesTable();
  });

  await loadFines();
  setupAddFineForm();
});

// Shows every loan belonging to the picked member as its own clickable
// row - a member can have more than one loan out at once, and this is also
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

  // Rendered as actual .btn elements rather than .list-group-item - the
  // list-group styling was getting flattened by the site's dark theme CSS,
  // so these showed up as plain unstyled text with no indication they were
  // clickable at all. Buttons are styled correctly everywhere else in this
  // project, so this sidesteps that gap. The text label under the buttons
  // makes the selection state explicit instead of relying on a color change
  // that might also get swallowed by the theme.
  container.innerHTML = matchingLoans.map(loan => {
    const title = loan.bookCopy?.book?.bookTitle ?? `Copy #${loan.bookCopyId}`;
    return `
      <button type="button" class="btn btn-outline-primary text-start w-100 mb-1 fine-loan-option" data-loan-id="${loan.loanId}">
        Loan #${loan.loanId} - ${title}, due ${new Date(loan.loanDueDate).toLocaleDateString()} (${loan.loanStatus})
      </button>
    `;
  }).join("") + `<p class="small text-muted mt-1 mb-0" id="fineLoanSelectedLabel">No loan selected yet - click one above.</p>`;

  container.querySelectorAll(".fine-loan-option").forEach(btn => {
    btn.addEventListener("click", () => {
      container.querySelectorAll(".fine-loan-option").forEach(b => {
        b.classList.remove("btn-primary");
        b.classList.add("btn-outline-primary");
      });
      btn.classList.remove("btn-outline-primary");
      btn.classList.add("btn-primary");

      selectedLoanId = btn.dataset.loanId;
      document.getElementById("fineLoanId").value = selectedLoanId;
      document.getElementById("fineLoanSelectedLabel").textContent = `Selected: Loan #${selectedLoanId}`;
    });
  });
}

async function loadFines() {
  try {
    allFines = await getAllFines();
  } catch (err) {
    document.getElementById("finesTableBody").innerHTML = `<tr><td colspan="7"><div class="alert alert-danger mb-0">${err.message}</div></td></tr>`;
    return;
  }
  renderFinesTable();
}

// Cross-references a fine against the already-loaded loan list to get the
// book/borrower details GetAllFines itself doesn't reliably return.
function getFineDetails(fine) {
  const matchedLoan = loansForPicker.find(l => String(l.loanId) === String(fine.loanId));
  const book = matchedLoan?.bookCopy?.book ?? fine.loan?.bookCopy?.book;
  const user = matchedLoan?.user ?? fine.loan?.user;

  return {
    title: book?.bookTitle ?? `Copy #${matchedLoan?.bookCopyId ?? fine.loan?.bookCopyId ?? "?"}`,
    edition: book?.bookEdition,
    dueDate: matchedLoan?.loanDueDate ?? fine.loan?.loanDueDate,
    borrowerName: user ? `${user.firstName} ${user.lastName}` : null,
    borrowerId: user ? extractUserRecordId(user) : null
  };
}

function renderFinesTable() {
  const tbody = document.getElementById("finesTableBody");
  const emptyMessage = document.getElementById("finesEmptyMessage");
  const loadMoreBtn = document.getElementById("loadMoreFinesBtn");
  const filterLabel = document.getElementById("finesFilterLabel");
  const clearBtn = document.getElementById("clearFineFilterBtn");

  let filtered = finesFilterUserId
    ? allFines.filter(fine => String(getFineDetails(fine).borrowerId) === String(finesFilterUserId))
    : allFines.slice();

  filtered.sort((a, b) => {
    const diff = new Date(a.fineIssueDate) - new Date(b.fineIssueDate);
    return finesSortOrder === "newest" ? -diff : diff;
  });

  const visible = filtered.slice(0, finesVisibleCount);

  if (finesFilterUserId) {
    const filteredUser = usersForPicker.find(u => String(extractUserRecordId(u)) === String(finesFilterUserId));
    filterLabel.textContent = filteredUser ? `Showing fines for ${filteredUser.firstName} ${filteredUser.lastName}` : "";
    clearBtn.classList.remove("d-none");
  } else {
    filterLabel.textContent = "";
    clearBtn.classList.add("d-none");
  }

  if (visible.length === 0) {
    tbody.innerHTML = "";
    emptyMessage.classList.remove("d-none");
  } else {
    emptyMessage.classList.add("d-none");
    tbody.innerHTML = visible.map(renderFineTableRow).join("");
  }

  loadMoreBtn.classList.toggle("d-none", filtered.length <= visible.length);

  attachFineHandlers();
  updateTotalUnpaid(filtered);
}

function renderFineTableRow(fine) {
  const details = getFineDetails(fine);
  const statuses = ["Paid", "Unpaid", "Dismissed"];
  const editionText = details.edition ? ` (Ed. ${details.edition})` : "";

  return `
    <tr data-fine-id="${fine.fineId}">
      <td>${details.title}${editionText}</td>
      <td>${details.borrowerName ?? "Unknown"}${details.borrowerId ? ` (ID ${details.borrowerId})` : ""}</td>
      <td>$${fine.fineAmount.toFixed(2)}</td>
      <td>${new Date(fine.fineIssueDate).toLocaleDateString()}</td>
      <td>${details.dueDate ? new Date(details.dueDate).toLocaleDateString() : "N/A"}</td>
      <td>
        <select class="form-select form-select-sm fine-status-select">
          ${statuses.map(s => `<option value="${s}" ${s === fine.status ? "selected" : ""}>${s}</option>`).join("")}
        </select>
      </td>
      <td class="text-end">
        <button class="btn btn-sm btn-outline-primary update-fine-btn">Update</button>
        <button class="btn btn-sm btn-outline-danger delete-fine-btn">Delete</button>
      </td>
    </tr>
  `;
}

// Global total (all members, all fines) uses the dedicated backend endpoint
// when nothing's filtered; once filtered to one member, it's computed
// client-side from that member's currently-sorted list instead, since the
// backend endpoint only ever answers "everyone."
async function updateTotalUnpaid(currentlyShownList) {
  const container = document.getElementById("totalUnpaid");

  if (finesFilterUserId) {
    const unpaidTotal = currentlyShownList
      .filter(f => f.status === "Unpaid")
      .reduce((sum, f) => sum + f.fineAmount, 0);
    container.innerHTML = `<strong>Unpaid total for this member:</strong> $${unpaidTotal.toFixed(2)}`;
    return;
  }

  try {
    const result = await getTotalUnpaidFines();
    container.innerHTML = `<strong>Total unpaid across all members:</strong> $${result.totalUnpaid.toFixed(2)}`;
  } catch (err) {
    container.innerHTML = "";
  }
}

function attachFineHandlers() {
  document.querySelectorAll(".update-fine-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      const row = btn.closest("[data-fine-id]");
      const newStatus = row.querySelector(".fine-status-select").value;
      try {
        await updateFineStatus(row.dataset.fineId, newStatus);
        await loadFines();
      } catch (err) {
        alert("Could not update fine: " + err.message);
      }
    });
  });

  document.querySelectorAll(".delete-fine-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      const row = btn.closest("[data-fine-id]");
      if (!confirm("Delete this fine?")) return;
      try {
        await deleteFine(row.dataset.fineId);
        await loadFines();
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
      await loadFines();
    } catch (err) {
      alert("Could not issue fine: " + err.message);
    }
  });
}
