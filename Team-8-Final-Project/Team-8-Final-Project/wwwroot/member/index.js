// ============================================================
// index.js — powers the book catalog list page
// ============================================================

document.addEventListener("DOMContentLoaded", loadBooks);

async function loadBooks() {
  const container = document.getElementById("booksContainer");

  try {
    const books = await getBooks();

    if (books.length === 0) {
      container.innerHTML = '<p class="text-muted">No books yet.</p>';
      return;
    }

    container.innerHTML = books.map(renderBookCard).join("");
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">Could not load books: ${err.message}</div>`;
  }
}

function renderBookCard(book) {
  return `
    <div class="col-12 col-md-6 col-lg-4 mb-4">
      <div class="card h-100">
        <div class="card-body d-flex flex-column">
          <h5 class="card-title">${book.bookTitle}</h5>
          <p class="card-text text-muted mb-3">ISBN: ${book.isbn}</p>
          <a href="book-details.html?id=${book.bookId}" class="btn btn-primary mt-auto">View details</a>
        </div>
      </div>
    </div>
  `;
}
