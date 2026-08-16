// ============================================================
// book-details.js — powers book-details.html
// Two independent chunks: the book's own info (top of page), and its
// reviews (view all + write your own + edit/delete your own).
// ============================================================

document.addEventListener("DOMContentLoaded", () => {
  const id = getIdFromQueryString();
  const container = document.getElementById("bookDetails");

  if (!id) {
    container.innerHTML = '<div class="alert alert-danger">No book id provided in the URL.</div>';
    return;
  }

  loadBookDetails(id);
  loadReviews(id);
  setupReviewForm(id);
});

function getIdFromQueryString() {
  const params = new URLSearchParams(window.location.search);
  return params.get("id");
}

// ---- Book info ----

async function loadBookDetails(id) {
  const container = document.getElementById("bookDetails");

  try {
    // Fetched together: there's no "get copies for this one book" endpoint,
    // so this pulls every book copy in the library and filters down to this
    // book's, same simplification used in my-loans.js/my-fines.js.
    const [book, allCopies] = await Promise.all([getBook(id), getAllBookCopies()]);
    const role = getUserRole();
    const isStaff = role === "Librarian" || role === "Admin";

    const copiesForThisBook = allCopies.filter(c => String(c.bookId) === String(book.bookId));
    const availableCount = copiesForThisBook.filter(c => c.availabilityStatus === "Available").length;
    const totalCount = copiesForThisBook.length;

    const availabilityHtml = totalCount === 0
      ? `<span class="badge bg-secondary">No copies in the system yet</span>`
      : availableCount > 0
        ? `<span class="badge bg-success">${availableCount} of ${totalCount} ${totalCount === 1 ? "copy" : "copies"} available</span>`
        : `<span class="badge bg-danger">All ${totalCount} ${totalCount === 1 ? "copy is" : "copies are"} currently checked out or reserved</span>`;

    container.innerHTML = `
      <h1>${book.bookTitle}</h1>
      <p class="text-muted">ISBN: ${book.isbn} | Edition: ${book.bookEdition ?? "N/A"} | ${book.bookLanguage}</p>
      <p>${availabilityHtml}</p>
      <button type="button" class="btn btn-success me-2" id="reserveBtn">Reserve this book</button>
      ${isStaff ? `
        <a href="book-form.html?id=${book.bookId}" class="btn btn-outline-primary me-2">Edit</a>
        <button type="button" class="btn btn-outline-danger" id="deleteBtn">Delete</button>
      ` : ""}
      ${availableCount === 0 && totalCount > 0 ? `
        <p class="text-muted mt-2 mb-0">
          This book is reservable as a waitlist request even while unavailable, but there's currently
          no way to show exactly when a copy frees up - that needs LoanController's GetLoansByBookCopy
          endpoint opened up to members (it's Librarian/Admin-only right now), or a due-date lookup
          added specifically for this. Ask a staff member in the meantime.
        </p>
      ` : ""}
    `;

    // Reserving is open to any logged-in user (Member or staff) - actually
    // checking a book out to a loan is Librarian/Admin-only on the backend
    // (AddLoan), so there's no self-checkout button here on purpose. This
    // just places a hold; staff turns it into a loan via staff/circulation.html.
    document.getElementById("reserveBtn").addEventListener("click", async () => {
      try {
        await createReservation({
          bookId: book.bookId,
          userId: getUserId(),
          reservationDate: new Date().toISOString()
        });
        // Redirect to the reservations list instead of an alert() popup, so
        // there's an actual page confirming it - my-reservations.js shows a
        // banner up top when it sees ?justReserved=1.
        window.location.href = "my-reservations.html?justReserved=1";
      } catch (err) {
        alert("Could not reserve this book: " + err.message);
      }
    });

    if (isStaff) {
      document.getElementById("deleteBtn").addEventListener("click", async () => {
        const confirmed = confirm(`Delete "${book.bookTitle}"? This can't be undone.`);
        if (!confirmed) return;

        await deleteBook(book.bookId);
        window.location.href = "dashboard.html";
      });
    }
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">${err.message}</div>`;
  }
}

// ---- Reviews ----

async function loadReviews(bookId) {
  const container = document.getElementById("reviewsContainer");
  const avgContainer = document.getElementById("averageRating");
  const currentUserId = getUserId();
  const role = getUserRole();
  const isStaff = role === "Librarian" || role === "Admin";

  try {
    const reviews = await getReviewsByBook(bookId);

    if (reviews.length === 0) {
      container.innerHTML = '<p class="text-muted">No reviews yet - be the first!</p>';
      avgContainer.innerHTML = "";
      return;
    }

    container.innerHTML = reviews.map(r => renderReviewCard(r, currentUserId, isStaff)).join("");
    attachReviewButtonHandlers(bookId);

    // Average rating has its own endpoint (GetAverageRating), so ask the API
    // for it instead of computing it ourselves from the list we already have.
    const avg = await getAverageRating(bookId);
    avgContainer.innerHTML = `<strong>Average rating:</strong> ${avg.averageRating.toFixed(1)} / 5`;
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">Could not load reviews: ${err.message}</div>`;
  }
}

// currentUserId/isStaff decide whether the Edit/Delete buttons show up on a
// given card - this mirrors the exact same isOwner-or-isStaff check the
// backend does in ReviewController, just so the buttons don't appear only
// to get rejected with a 403 when clicked.
function renderReviewCard(review, currentUserId, isStaff) {
  // review.user comes from the controller's .Include(r => r.User)
  const reviewerName = review.user ? `${review.user.firstName} ${review.user.lastName}` : "Unknown user";
  const isOwner = String(review.userId) === String(currentUserId);
  const canEdit = isOwner || isStaff;

  return `
    <div class="card mb-3" data-review-id="${review.reviewId}">
      <div class="card-body">
        <h6 class="card-title">${reviewerName} &middot; ${review.rating} / 5</h6>
        <p class="card-text">${review.comment ?? ""}</p>
        <p class="text-muted small mb-2">${new Date(review.reviewDate).toLocaleDateString()}</p>
        ${canEdit ? `
          <button type="button" class="btn btn-sm btn-outline-primary edit-review-btn">Edit</button>
          <button type="button" class="btn btn-sm btn-outline-danger delete-review-btn">Delete</button>
        ` : ""}
      </div>
    </div>
  `;
}

// Since review cards are rebuilt every time loadReviews() runs, their button
// listeners have to be re-attached each time too - that's why this is a
// separate function called right after the innerHTML swap, not inline HTML.
function attachReviewButtonHandlers(bookId) {
  document.querySelectorAll(".delete-review-btn").forEach(btn => {
    btn.addEventListener("click", async (event) => {
      const card = event.target.closest("[data-review-id]");
      const reviewId = card.dataset.reviewId;

      const confirmed = confirm("Delete this review?");
      if (!confirmed) return;

      try {
        await deleteReview(reviewId);
        loadReviews(bookId);
      } catch (err) {
        alert("Could not delete review: " + err.message);
      }
    });
  });

  document.querySelectorAll(".edit-review-btn").forEach(btn => {
    btn.addEventListener("click", (event) => {
      const card = event.target.closest("[data-review-id]");
      const reviewId = card.dataset.reviewId;
      const currentComment = card.querySelector(".card-text").textContent;

      // Using prompt() here instead of a proper inline edit form - simplest
      // possible way to collect two values, worth upgrading later once the
      // CSS pass happens and there's a nicer place to put a small form.
      const newRating = prompt("New rating (1-5):");
      if (newRating === null) return; // user clicked Cancel

      const newComment = prompt("New comment:", currentComment);
      if (newComment === null) return; // user clicked Cancel

      updateReview(reviewId, { rating: Number(newRating), comment: newComment })
        .then(() => loadReviews(bookId))
        .catch(err => alert("Could not update review: " + err.message));
    });
  });
}

function setupReviewForm(bookId) {
  const form = document.getElementById("addReviewForm");

  form.addEventListener("submit", async (event) => {
    event.preventDefault(); // stop the browser's default full-page form submit

    const rating = Number(document.getElementById("rating").value);
    const comment = document.getElementById("comment").value;

    try {
      await addReview({ rating, comment, bookId: Number(bookId) });
      form.reset();
      loadReviews(bookId);
    } catch (err) {
      alert("Could not submit review: " + err.message);
    }
  });
}
