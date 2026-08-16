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
    const book = await getBook(id);
    const role = getUserRole();
    const isStaff = role === "Librarian" || role === "Admin";

    container.innerHTML = `
      <h1>${book.bookTitle}</h1>
      <p class="text-muted">ISBN: ${book.isbn} | Edition: ${book.bookEdition ?? "N/A"} | ${book.bookLanguage}</p>
      ${isStaff ? `
        <a href="book-form.html?id=${book.bookId}" class="btn btn-outline-primary me-2">Edit</a>
        <button type="button" class="btn btn-outline-danger" id="deleteBtn">Delete</button>
      ` : ""}
    `;

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
