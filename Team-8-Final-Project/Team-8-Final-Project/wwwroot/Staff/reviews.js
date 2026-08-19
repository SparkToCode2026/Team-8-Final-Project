// ============================================================
// reviews.js — powers staff/reviews.html
// ============================================================

let booksForPicker = [];
let reviewBookPicker;

document.addEventListener("DOMContentLoaded", async () => {
  try {
    booksForPicker = await getBooks();
  } catch (err) {
    console.error("Could not load books for the search picker:", err);
  }

  reviewBookPicker = createSearchPicker({
    containerId: "reviewBookPicker",
    items: () => booksForPicker,
    getId: b => b.bookId,
    getLabel: b => `${b.bookTitle} (Edition ${b.bookEdition ?? "N/A"})`,
    placeholder: "Search book by title...",
    onSelect: () => loadReviewsForBook() // load immediately on pick, no need to also click the button
  });

  document.getElementById("lookupForm").addEventListener("submit", (event) => {
    event.preventDefault();
    loadReviewsForBook();
  });
});

async function loadReviewsForBook() {
  const container = document.getElementById("reviewsContainer");
  const bookId = reviewBookPicker.getSelectedId();
  const highOnly = document.getElementById("highRatingOnly").checked;

  if (!bookId) {
    container.innerHTML = '<p class="text-muted">Search for the book by title and select it from the list.</p>';
    return;
  }

  try {
    // FilterHighRatingReviews doesn't include the reviewer's info the way
    // GetReviewsByBook does, so renderReviewRow falls back to a plain
    // "User #<id>" label when review.user isn't there.
    const reviews = highOnly
      ? await filterHighRatingReviews(bookId)
      : await getReviewsByBook(bookId);

    container.innerHTML = reviews.length
      ? reviews.map(renderReviewRow).join("")
      : '<p class="text-muted">No reviews found for that book.</p>';

    attachReviewHandlers();
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">${err.message}</div>`;
  }
}

function renderReviewRow(review) {
  const reviewerName = review.user ? `${review.user.firstName} ${review.user.lastName}` : `User #${review.userId}`;

  return `
    <div class="card mb-2" data-review-id="${review.reviewId}">
      <div class="card-body d-flex justify-content-between align-items-center">
        <div>
          <strong>${reviewerName} &middot; ${review.rating} / 5</strong>
          <p class="mb-0 text-muted">${review.comment ?? ""}</p>
        </div>
        <button class="btn btn-sm btn-outline-danger delete-review-btn">Delete</button>
      </div>
    </div>
  `;
}

function attachReviewHandlers() {
  document.querySelectorAll(".delete-review-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      const card = btn.closest("[data-review-id]");
      if (!confirm("Delete this review?")) return;
      try {
        await deleteReview(card.dataset.reviewId);
        loadReviewsForBook();
      } catch (err) {
        alert("Could not delete review: " + err.message);
      }
    });
  });
}
