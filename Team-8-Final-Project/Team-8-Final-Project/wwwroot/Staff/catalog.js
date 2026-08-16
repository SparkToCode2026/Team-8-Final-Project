// ============================================================
// catalog.js — powers staff/catalog.html
//
// Five near-identical sections (Books, Copies, Authors, Categories,
// Shelves): load a list into cards, wire up an "Add" form, wire up a
// Delete button on each card. Editing isn't built here yet - only add
// and delete - to keep this page's scope manageable; add it later the
// same way book-details.js's review edit works (a small prompt(), or a
// proper inline form once the CSS pass happens).
//
// Publishers isn't included yet - waiting on PublishersController.cs.
// ============================================================

document.addEventListener("DOMContentLoaded", () => {
  loadBooks();
  loadCopies();
  loadAuthors();
  loadPublishers();
  loadCategories();
  loadShelves();

  setupAddBookForm();
  setupAddCopyForm();
  setupAddAuthorForm();
  setupAddPublisherForm();
  setupAddCategoryForm();
  setupAddShelfForm();
});

// ---- Books ----

async function loadBooks() {
  const container = document.getElementById("booksContainer");
  try {
    const books = await getBooks();
    container.innerHTML = books.length ? books.map(renderBookRow).join("") : '<p class="text-muted">No books yet.</p>';
    document.querySelectorAll(".delete-book-btn").forEach(btn => {
      btn.addEventListener("click", async () => {
        if (!confirm("Delete this book?")) return;
        try { await deleteBook(btn.dataset.id); loadBooks(); }
        catch (err) { alert("Could not delete: " + err.message); }
      });
    });
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">${err.message}</div>`;
  }
}

function renderBookRow(book) {
  return `
    <div class="card mb-2">
      <div class="card-body d-flex justify-content-between align-items-center">
        <div>
          <strong>${book.bookTitle}</strong>
          <span class="text-muted"> - ISBN ${book.isbn}, ${book.bookLanguage}, ${book.year ?? "N/A"}</span>
        </div>
        <button class="btn btn-sm btn-outline-danger delete-book-btn" data-id="${book.bookId}">Delete</button>
      </div>
    </div>
  `;
}

function setupAddBookForm() {
  document.getElementById("addBookForm").addEventListener("submit", async (event) => {
    event.preventDefault();

    const authorIds = document.getElementById("bookAuthorIds").value
      .split(",").map(s => s.trim()).filter(Boolean).map(Number);

    const book = {
      isbn: document.getElementById("bookIsbn").value,
      bookTitle: document.getElementById("bookTitle").value,
      bookEdition: Number(document.getElementById("bookEdition").value),
      bookLanguage: document.getElementById("bookLanguage").value,
      year: Number(document.getElementById("bookYear").value),
      publisherId: Number(document.getElementById("bookPublisherId").value),
      categoryId: Number(document.getElementById("bookCategoryId").value),
      authorIds
    };

    try {
      await createBook(book);
      event.target.reset();
      loadBooks();
    } catch (err) {
      alert("Could not add book: " + err.message);
    }
  });
}

// ---- Book Copies ----

async function loadCopies() {
  const container = document.getElementById("copiesContainer");
  try {
    const copies = await getAllBookCopies();
    container.innerHTML = copies.length ? copies.map(renderCopyRow).join("") : '<p class="text-muted">No book copies yet.</p>';
    document.querySelectorAll(".delete-copy-btn").forEach(btn => {
      btn.addEventListener("click", async () => {
        if (!confirm("Delete this book copy?")) return;
        try { await deleteBookCopy(btn.dataset.id); loadCopies(); }
        catch (err) { alert("Could not delete: " + err.message); }
      });
    });
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">${err.message}</div>`;
  }
}

function renderCopyRow(copy) {
  const title = copy.book?.bookTitle ?? "Unknown book";
  return `
    <div class="card mb-2">
      <div class="card-body d-flex justify-content-between align-items-center">
        <div>
          <strong>${copy.barcode}</strong>
          <span class="text-muted"> - ${title}, ${copy.condition}, ${copy.availabilityStatus}</span>
        </div>
        <button class="btn btn-sm btn-outline-danger delete-copy-btn" data-id="${copy.bookCopyId}">Delete</button>
      </div>
    </div>
  `;
}

function setupAddCopyForm() {
  document.getElementById("addCopyForm").addEventListener("submit", async (event) => {
    event.preventDefault();

    const copy = {
      barcode: document.getElementById("copyBarcode").value,
      condition: document.getElementById("copyCondition").value,
      availabilityStatus: document.getElementById("copyAvailability").value,
      copyPrice: Number(document.getElementById("copyPrice").value),
      bookId: Number(document.getElementById("copyBookId").value),
      shelfId: Number(document.getElementById("copyShelfId").value)
    };

    try {
      await addBookCopy(copy);
      event.target.reset();
      loadCopies();
    } catch (err) {
      alert("Could not add book copy: " + err.message);
    }
  });
}

// ---- Authors ----

async function loadAuthors() {
  const container = document.getElementById("authorsContainer");
  try {
    const authors = await getAuthors();
    container.innerHTML = authors.length ? authors.map(renderAuthorRow).join("") : '<p class="text-muted">No authors yet.</p>';
    document.querySelectorAll(".delete-author-btn").forEach(btn => {
      btn.addEventListener("click", async () => {
        if (!confirm("Delete this author?")) return;
        try { await deleteAuthor(btn.dataset.id); loadAuthors(); }
        catch (err) { alert("Could not delete: " + err.message); }
      });
    });
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">${err.message}</div>`;
  }
}

function renderAuthorRow(author) {
  return `
    <div class="card mb-2">
      <div class="card-body d-flex justify-content-between align-items-center">
        <div>
          <strong>${author.firstName} ${author.lastName}</strong>
          <span class="text-muted"> - ${author.nationality ?? "N/A"}</span>
        </div>
        <button class="btn btn-sm btn-outline-danger delete-author-btn" data-id="${author.authorId}">Delete</button>
      </div>
    </div>
  `;
}

function setupAddAuthorForm() {
  document.getElementById("addAuthorForm").addEventListener("submit", async (event) => {
    event.preventDefault();

    const author = {
      firstName: document.getElementById("authorFirstName").value,
      lastName: document.getElementById("authorLastName").value,
      email: document.getElementById("authorEmail").value,
      nationality: document.getElementById("authorNationality").value,
      biography: document.getElementById("authorBio").value
    };

    try {
      await addAuthor(author);
      event.target.reset();
      loadAuthors();
    } catch (err) {
      alert("Could not add author: " + err.message);
    }
  });
}

// ---- Publishers ----

async function loadPublishers() {
  const container = document.getElementById("publishersContainer");
  try {
    const publishers = await getPublishers();
    container.innerHTML = publishers.length ? publishers.map(renderPublisherRow).join("") : '<p class="text-muted">No publishers yet.</p>';
    document.querySelectorAll(".delete-publisher-btn").forEach(btn => {
      btn.addEventListener("click", async () => {
        if (!confirm("Delete this publisher?")) return;
        try {
          await deletePublisher(btn.dataset.id);
          loadPublishers();
        } catch (err) {
          // PublishersController returns 409 Conflict if the publisher still has books
          alert("Could not delete: " + err.message);
        }
      });
    });
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">${err.message}</div>`;
  }
}

function renderPublisherRow(publisher) {
  return `
    <div class="card mb-2">
      <div class="card-body d-flex justify-content-between align-items-center">
        <div>
          <strong>${publisher.publisherName}</strong>
          <span class="text-muted"> - ${publisher.publisherCode}, ${publisher.publisherEmail ?? "no email"}</span>
        </div>
        <button class="btn btn-sm btn-outline-danger delete-publisher-btn" data-id="${publisher.publisherId}">Delete</button>
      </div>
    </div>
  `;
}

function setupAddPublisherForm() {
  document.getElementById("addPublisherForm").addEventListener("submit", async (event) => {
    event.preventDefault();

    const publisher = {
      publisherCode: document.getElementById("publisherCode").value,
      publisherName: document.getElementById("publisherName").value,
      publisherAddress: document.getElementById("publisherAddress").value,
      publisherLandlineNo: document.getElementById("publisherLandline").value,
      publisherEmail: document.getElementById("publisherEmail").value
    };

    try {
      await addPublisher(publisher);
      event.target.reset();
      loadPublishers();
    } catch (err) {
      alert("Could not add publisher: " + err.message);
    }
  });
}

// ---- Categories ----

async function loadCategories() {
  const container = document.getElementById("categoriesContainer");
  try {
    const categories = await getCategories();
    container.innerHTML = categories.length ? categories.map(renderCategoryRow).join("") : '<p class="text-muted">No categories yet.</p>';
    document.querySelectorAll(".delete-category-btn").forEach(btn => {
      btn.addEventListener("click", async () => {
        if (!confirm("Delete this category?")) return;
        try { await deleteCategory(btn.dataset.id); loadCategories(); }
        catch (err) { alert("Could not delete: " + err.message); }
      });
    });
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">${err.message}</div>`;
  }
}

function renderCategoryRow(category) {
  return `
    <div class="card mb-2">
      <div class="card-body d-flex justify-content-between align-items-center">
        <div>
          <strong>${category.categoryName}</strong>
          <span class="text-muted"> - ${category.categoryDescription ?? ""}</span>
        </div>
        <button class="btn btn-sm btn-outline-danger delete-category-btn" data-id="${category.categoryId}">Delete</button>
      </div>
    </div>
  `;
}

function setupAddCategoryForm() {
  document.getElementById("addCategoryForm").addEventListener("submit", async (event) => {
    event.preventDefault();

    const category = {
      categoryName: document.getElementById("categoryName").value,
      categoryDescription: document.getElementById("categoryDescription").value
    };

    try {
      await addCategory(category);
      event.target.reset();
      loadCategories();
    } catch (err) {
      alert("Could not add category: " + err.message);
    }
  });
}

// ---- Shelves ----

async function loadShelves() {
  const container = document.getElementById("shelvesContainer");
  try {
    const shelves = await getShelves();
    container.innerHTML = shelves.length ? shelves.map(renderShelfRow).join("") : '<p class="text-muted">No shelves yet.</p>';
    document.querySelectorAll(".delete-shelf-btn").forEach(btn => {
      btn.addEventListener("click", async () => {
        if (!confirm("Delete this shelf?")) return;
        try { await deleteShelf(btn.dataset.id); loadShelves(); }
        catch (err) { alert("Could not delete: " + err.message); }
      });
    });
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">${err.message}</div>`;
  }
}

function renderShelfRow(shelf) {
  return `
    <div class="card mb-2">
      <div class="card-body d-flex justify-content-between align-items-center">
        <div>
          <strong>${shelf.shelfCode}</strong>
          <span class="text-muted"> - ${shelf.section}, Floor ${shelf.floorNumber}</span>
        </div>
        <button class="btn btn-sm btn-outline-danger delete-shelf-btn" data-id="${shelf.shelfId}">Delete</button>
      </div>
    </div>
  `;
}

function setupAddShelfForm() {
  document.getElementById("addShelfForm").addEventListener("submit", async (event) => {
    event.preventDefault();

    const shelf = {
      shelfCode: document.getElementById("shelfCode").value,
      section: document.getElementById("shelfSection").value,
      floorNumber: Number(document.getElementById("shelfFloor").value)
    };

    try {
      await addShelf(shelf);
      event.target.reset();
      loadShelves();
    } catch (err) {
      alert("Could not add shelf: " + err.message);
    }
  });
}
