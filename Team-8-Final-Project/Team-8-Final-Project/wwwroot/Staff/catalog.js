// ============================================================
// catalog.js — powers staff/catalog.html
//
// Six sections (Books, Copies, Authors, Publishers, Categories, Shelves),
// each following the same pattern: load a list into cards (showing each
// entry's ID), wire up an "Add" form, and give every card an Edit button
// (swaps the card into an inline form using the same fields as Add,
// pre-filled with current values) and a Delete button. Success actions
// show a fading confirmation banner at the top of the page instead of
// silently doing nothing visible.
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

// Shared success banner for every Add/Edit/Delete action on this page.
function showCatalogBanner(message) {
  const banner = document.getElementById("catalogBanner");
  banner.innerHTML = `<div class="alert alert-success">${message}</div>`;
  setTimeout(() => { banner.innerHTML = ""; }, 4000);
}

// ---- Books ----

let booksData = [];

async function loadBooks() {
  const container = document.getElementById("booksContainer");
  try {
    booksData = await getBooks();
    container.innerHTML = booksData.length ? booksData.map(renderBookRow).join("") : '<p class="text-muted">No books yet.</p>';
    attachBookHandlers();
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">${err.message}</div>`;
  }
}

function renderBookRow(book) {
  return `
    <div class="card mb-2" data-id="${book.bookId}">
      <div class="card-body d-flex justify-content-between align-items-center flex-wrap gap-2">
        <div>
          <span class="badge bg-secondary">ID ${book.bookId}</span>
          <strong>${book.bookTitle}</strong>
          <span class="text-muted"> - ISBN ${book.isbn}, ${book.bookLanguage}, ${book.year ?? "N/A"}</span>
        </div>
        <div class="d-flex gap-2">
          <button class="btn btn-sm btn-outline-primary edit-book-btn" data-id="${book.bookId}">Edit</button>
          <button class="btn btn-sm btn-outline-danger delete-book-btn" data-id="${book.bookId}">Delete</button>
        </div>
      </div>
    </div>
  `;
}

function attachBookHandlers() {
  document.querySelectorAll(".edit-book-btn").forEach(btn => {
    btn.addEventListener("click", () => startEditBook(btn.dataset.id));
  });

  document.querySelectorAll(".delete-book-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      if (!confirm("Delete this book?")) return;
      try {
        await deleteBook(btn.dataset.id);
        loadBooks();
        showCatalogBanner("Book deleted.");
      }
      catch (err) { alert("Could not delete: " + err.message); }
    });
  });
}

function startEditBook(id) {
  const book = booksData.find(b => String(b.bookId) === String(id));
  const card = document.querySelector(`#booksContainer [data-id="${id}"]`);
  const authorIds = (book.authors || []).map(a => a.authorId).join(", ");
  const publisherId = book.publisherId ?? book.publisher?.publisherId ?? "";
  const categoryId = book.categoryId ?? book.category?.categoryId ?? "";

  card.innerHTML = `
    <div class="card-body">
      <span class="badge bg-secondary mb-2">Editing ID ${book.bookId}</span>
      <div class="row g-2">
        <div class="col-md-3"><input type="text" class="form-control" id="editBookIsbn-${id}" placeholder="ISBN" value="${book.isbn ?? ""}"></div>
        <div class="col-md-3"><input type="text" class="form-control" id="editBookTitle-${id}" placeholder="Title" value="${book.bookTitle ?? ""}"></div>
        <div class="col-md-2"><input type="number" class="form-control" id="editBookEdition-${id}" placeholder="Edition" value="${book.bookEdition ?? ""}"></div>
        <div class="col-md-2"><input type="text" class="form-control" id="editBookLanguage-${id}" placeholder="Language" value="${book.bookLanguage ?? ""}"></div>
        <div class="col-md-2"><input type="number" class="form-control" id="editBookYear-${id}" placeholder="Year" value="${book.year ?? ""}"></div>
        <div class="col-md-3"><input type="number" class="form-control" id="editBookPublisherId-${id}" placeholder="Publisher Id" value="${publisherId}"></div>
        <div class="col-md-3"><input type="number" class="form-control" id="editBookCategoryId-${id}" placeholder="Category Id" value="${categoryId}"></div>
        <div class="col-md-4"><input type="text" class="form-control" id="editBookAuthorIds-${id}" placeholder="Author Ids, comma separated" value="${authorIds}"></div>
        <div class="col-md-2 d-flex gap-2">
          <button class="btn btn-sm btn-success save-book-btn" data-id="${id}">Save</button>
          <button class="btn btn-sm btn-secondary cancel-book-btn" data-id="${id}">Cancel</button>
        </div>
      </div>
    </div>
  `;

  card.querySelector(".save-book-btn").addEventListener("click", async () => {
    const updated = {
      isbn: document.getElementById(`editBookIsbn-${id}`).value,
      bookTitle: document.getElementById(`editBookTitle-${id}`).value,
      bookEdition: Number(document.getElementById(`editBookEdition-${id}`).value),
      bookLanguage: document.getElementById(`editBookLanguage-${id}`).value,
      year: Number(document.getElementById(`editBookYear-${id}`).value),
      publisherId: Number(document.getElementById(`editBookPublisherId-${id}`).value),
      categoryId: Number(document.getElementById(`editBookCategoryId-${id}`).value),
      authorIds: document.getElementById(`editBookAuthorIds-${id}`).value.split(",").map(s => s.trim()).filter(Boolean).map(Number)
    };

    try {
      await updateBook(id, updated);
      loadBooks();
      showCatalogBanner(`"${updated.bookTitle}" was updated.`);
    } catch (err) {
      alert("Could not update book: " + err.message);
    }
  });

  card.querySelector(".cancel-book-btn").addEventListener("click", loadBooks);
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
      showCatalogBanner(`"${book.bookTitle}" was added.`);
    } catch (err) {
      alert("Could not add book: " + err.message);
    }
  });
}

// ---- Book Copies ----

let copiesData = [];

async function loadCopies() {
  const container = document.getElementById("copiesContainer");
  try {
    copiesData = await getAllBookCopies();
    container.innerHTML = copiesData.length ? copiesData.map(renderCopyRow).join("") : '<p class="text-muted">No book copies yet.</p>';
    attachCopyHandlers();
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">${err.message}</div>`;
  }
}

function renderCopyRow(copy) {
  const title = copy.book?.bookTitle ?? "Unknown book";
  return `
    <div class="card mb-2" data-id="${copy.bookCopyId}">
      <div class="card-body d-flex justify-content-between align-items-center flex-wrap gap-2">
        <div>
          <span class="badge bg-secondary">ID ${copy.bookCopyId}</span>
          <strong>${copy.barcode}</strong>
          <span class="text-muted"> - ${title}, ${copy.condition}, ${copy.availabilityStatus}</span>
        </div>
        <div class="d-flex gap-2">
          <button class="btn btn-sm btn-outline-primary edit-copy-btn" data-id="${copy.bookCopyId}">Edit</button>
          <button class="btn btn-sm btn-outline-danger delete-copy-btn" data-id="${copy.bookCopyId}">Delete</button>
        </div>
      </div>
    </div>
  `;
}

function attachCopyHandlers() {
  document.querySelectorAll(".edit-copy-btn").forEach(btn => {
    btn.addEventListener("click", () => startEditCopy(btn.dataset.id));
  });

  document.querySelectorAll(".delete-copy-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      if (!confirm("Delete this book copy?")) return;
      try {
        await deleteBookCopy(btn.dataset.id);
        loadCopies();
        showCatalogBanner("Book copy deleted.");
      }
      catch (err) { alert("Could not delete: " + err.message); }
    });
  });
}

function startEditCopy(id) {
  const copy = copiesData.find(c => String(c.bookCopyId) === String(id));
  const card = document.querySelector(`#copiesContainer [data-id="${id}"]`);
  const title = copy.book?.bookTitle ?? "Unknown book";
  const conditions = ["New", "Good", "Fair", "Poor"];
  const availabilities = ["Available", "OnLoan", "Reserved"];

  card.innerHTML = `
    <div class="card-body">
      <span class="badge bg-secondary mb-2">Editing ID ${copy.bookCopyId}</span>
      <span class="text-muted small ms-2">Book: ${title} (can't be changed after the copy is created - delete and re-add under the right Book Id instead)</span>
      <div class="row g-2 mt-1">
        <div class="col-md-3"><input type="text" class="form-control" id="editCopyBarcode-${id}" placeholder="Barcode" value="${copy.barcode ?? ""}"></div>
        <div class="col-md-2">
          <select class="form-select" id="editCopyCondition-${id}">
            ${conditions.map(c => `<option value="${c}" ${c === copy.condition ? "selected" : ""}>${c}</option>`).join("")}
          </select>
        </div>
        <div class="col-md-2">
          <select class="form-select" id="editCopyAvailability-${id}">
            ${availabilities.map(a => `<option value="${a}" ${a === copy.availabilityStatus ? "selected" : ""}>${a}</option>`).join("")}
          </select>
        </div>
        <div class="col-md-2"><input type="number" step="0.01" class="form-control" id="editCopyPrice-${id}" placeholder="Price" value="${copy.copyPrice ?? ""}"></div>
        <div class="col-md-1"><input type="number" class="form-control" id="editCopyShelfId-${id}" placeholder="Shelf Id" value="${copy.shelfId ?? copy.shelf?.shelfId ?? ""}"></div>
        <div class="col-md-2 d-flex gap-2">
          <button class="btn btn-sm btn-success save-copy-btn" data-id="${id}">Save</button>
          <button class="btn btn-sm btn-secondary cancel-copy-btn" data-id="${id}">Cancel</button>
        </div>
      </div>
    </div>
  `;

  card.querySelector(".save-copy-btn").addEventListener("click", async () => {
    const updated = {
      barcode: document.getElementById(`editCopyBarcode-${id}`).value,
      condition: document.getElementById(`editCopyCondition-${id}`).value,
      availabilityStatus: document.getElementById(`editCopyAvailability-${id}`).value,
      copyPrice: Number(document.getElementById(`editCopyPrice-${id}`).value),
      shelfId: Number(document.getElementById(`editCopyShelfId-${id}`).value)
    };

    try {
      await updateBookCopy(id, updated);
      loadCopies();
      showCatalogBanner(`Copy "${updated.barcode}" was updated.`);
    } catch (err) {
      alert("Could not update book copy: " + err.message);
    }
  });

  card.querySelector(".cancel-copy-btn").addEventListener("click", loadCopies);
}

// Type-ahead book picker for the Add Book Copies form - lets staff search by
// title instead of memorizing/looking up a numeric Book Id. Filters the same
// booksData array the Books tab already loaded, so no extra API call.
function setupBookSearchPicker() {
  const searchInput = document.getElementById("copyBookSearch");
  const hiddenId = document.getElementById("copyBookId");
  const resultsBox = document.getElementById("copyBookResults");

  searchInput.addEventListener("input", () => {
    hiddenId.value = ""; // typing again invalidates whatever was picked before
    const query = searchInput.value.trim().toLowerCase();

    if (!query) {
      resultsBox.innerHTML = "";
      return;
    }

    const matches = booksData
      .filter(b => b.bookTitle && b.bookTitle.toLowerCase().includes(query))
      .slice(0, 8);

    resultsBox.innerHTML = matches.map(b => `
      <button type="button" class="list-group-item list-group-item-action book-result" data-id="${b.bookId}" data-title="${b.bookTitle.replace(/"/g, "&quot;")}">
        ${b.bookTitle} <span class="text-muted">(Edition ${b.bookEdition ?? "N/A"})</span>
      </button>
    `).join("");

    resultsBox.querySelectorAll(".book-result").forEach(btn => {
      btn.addEventListener("click", () => {
        searchInput.value = btn.dataset.title;
        hiddenId.value = btn.dataset.id;
        resultsBox.innerHTML = "";
      });
    });
  });

  // Click anywhere outside the search box/results closes the dropdown
  document.addEventListener("click", (event) => {
    if (!event.target.closest("#copyBookSearch") && !event.target.closest("#copyBookResults")) {
      resultsBox.innerHTML = "";
    }
  });
}

function setupAddCopyForm() {
  setupBookSearchPicker();

  document.getElementById("addCopyForm").addEventListener("submit", async (event) => {
    event.preventDefault();

    const bookIdValue = document.getElementById("copyBookId").value;
    if (!bookIdValue) {
      alert("Search for the book by title and select it from the list before adding a copy.");
      return;
    }

    const barcodeInput = document.getElementById("copyBarcode").value.trim();
    const condition = document.getElementById("copyCondition").value;
    const availabilityStatus = document.getElementById("copyAvailability").value;
    const copyPrice = Number(document.getElementById("copyPrice").value);
    const bookId = Number(bookIdValue);
    const shelfId = Number(document.getElementById("copyShelfId").value);
    const quantity = Math.max(1, Number(document.getElementById("copyQuantity").value) || 1);

    // Barcode is required by the backend, but there's no scanner for the demo,
    // so when it's left blank this generates a short placeholder per copy -
    // same idea real library systems use (an auto-incrementing "accession
    // number") when there's no physical barcode to scan yet. If a barcode WAS
    // typed in and quantity is more than 1, a -1/-2/... suffix keeps each copy
    // unique instead of trying to insert the same barcode multiple times.
    function barcodeFor(index) {
      if (!barcodeInput) {
        return `AUTO-${bookId}-${Date.now().toString(36)}-${index + 1}`;
      }
      return quantity > 1 ? `${barcodeInput}-${index + 1}` : barcodeInput;
    }

    let successCount = 0;
    const errors = [];

    for (let i = 0; i < quantity; i++) {
      const copy = { barcode: barcodeFor(i), condition, availabilityStatus, copyPrice, bookId, shelfId };
      try {
        await addBookCopy(copy);
        successCount++;
      } catch (err) {
        errors.push(err.message);
      }
    }

    loadCopies();

    if (successCount > 0) {
      event.target.reset();
      document.getElementById("copyQuantity").value = "";
      showCatalogBanner(successCount === 1 ? "1 book copy was added." : `${successCount} book copies were added.`);
    }

    if (errors.length > 0) {
      alert(`${errors.length} of ${quantity} copies could not be added: ${errors[0]}`);
    }
  });
}

// ---- Authors ----

let authorsData = [];

async function loadAuthors() {
  const container = document.getElementById("authorsContainer");
  try {
    authorsData = await getAuthors();
    container.innerHTML = authorsData.length ? authorsData.map(renderAuthorRow).join("") : '<p class="text-muted">No authors yet.</p>';
    attachAuthorHandlers();
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">${err.message}</div>`;
  }
}

function renderAuthorRow(author) {
  return `
    <div class="card mb-2" data-id="${author.authorId}">
      <div class="card-body d-flex justify-content-between align-items-center flex-wrap gap-2">
        <div>
          <span class="badge bg-secondary">ID ${author.authorId}</span>
          <strong>${author.firstName} ${author.lastName}</strong>
          <span class="text-muted"> - ${author.nationality ?? "N/A"}</span>
        </div>
        <div class="d-flex gap-2">
          <button class="btn btn-sm btn-outline-primary edit-author-btn" data-id="${author.authorId}">Edit</button>
          <button class="btn btn-sm btn-outline-danger delete-author-btn" data-id="${author.authorId}">Delete</button>
        </div>
      </div>
    </div>
  `;
}

function attachAuthorHandlers() {
  document.querySelectorAll(".edit-author-btn").forEach(btn => {
    btn.addEventListener("click", () => startEditAuthor(btn.dataset.id));
  });

  document.querySelectorAll(".delete-author-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      if (!confirm("Delete this author?")) return;
      try {
        await deleteAuthor(btn.dataset.id);
        loadAuthors();
        showCatalogBanner("Author deleted.");
      }
      catch (err) { alert("Could not delete: " + err.message); }
    });
  });
}

function startEditAuthor(id) {
  const author = authorsData.find(a => String(a.authorId) === String(id));
  const card = document.querySelector(`#authorsContainer [data-id="${id}"]`);

  card.innerHTML = `
    <div class="card-body">
      <span class="badge bg-secondary mb-2">Editing ID ${author.authorId}</span>
      <div class="row g-2">
        <div class="col-md-2"><input type="text" class="form-control" id="editAuthorFirstName-${id}" placeholder="First name" value="${author.firstName ?? ""}"></div>
        <div class="col-md-2"><input type="text" class="form-control" id="editAuthorLastName-${id}" placeholder="Last name" value="${author.lastName ?? ""}"></div>
        <div class="col-md-3"><input type="email" class="form-control" id="editAuthorEmail-${id}" placeholder="Email" value="${author.email ?? ""}"></div>
        <div class="col-md-2"><input type="text" class="form-control" id="editAuthorNationality-${id}" placeholder="Nationality" value="${author.nationality ?? ""}"></div>
        <div class="col-md-3"><input type="text" class="form-control" id="editAuthorBio-${id}" placeholder="Biography" value="${author.biography ?? ""}"></div>
        <div class="col-md-2 d-flex gap-2">
          <button class="btn btn-sm btn-success save-author-btn" data-id="${id}">Save</button>
          <button class="btn btn-sm btn-secondary cancel-author-btn" data-id="${id}">Cancel</button>
        </div>
      </div>
    </div>
  `;

  card.querySelector(".save-author-btn").addEventListener("click", async () => {
    const updated = {
      firstName: document.getElementById(`editAuthorFirstName-${id}`).value,
      lastName: document.getElementById(`editAuthorLastName-${id}`).value,
      email: document.getElementById(`editAuthorEmail-${id}`).value,
      nationality: document.getElementById(`editAuthorNationality-${id}`).value,
      biography: document.getElementById(`editAuthorBio-${id}`).value
    };

    try {
      await updateAuthor(id, updated);
      loadAuthors();
      showCatalogBanner(`"${updated.firstName} ${updated.lastName}" was updated.`);
    } catch (err) {
      alert("Could not update author: " + err.message);
    }
  });

  card.querySelector(".cancel-author-btn").addEventListener("click", loadAuthors);
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
      showCatalogBanner(`"${author.firstName} ${author.lastName}" was added.`);
    } catch (err) {
      alert("Could not add author: " + err.message);
    }
  });
}

// ---- Publishers ----

let publishersData = [];

async function loadPublishers() {
  const container = document.getElementById("publishersContainer");
  try {
    publishersData = await getPublishers();
    container.innerHTML = publishersData.length ? publishersData.map(renderPublisherRow).join("") : '<p class="text-muted">No publishers yet.</p>';
    attachPublisherHandlers();
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">${err.message}</div>`;
  }
}

function renderPublisherRow(publisher) {
  return `
    <div class="card mb-2" data-id="${publisher.publisherId}">
      <div class="card-body d-flex justify-content-between align-items-center flex-wrap gap-2">
        <div>
          <span class="badge bg-secondary">ID ${publisher.publisherId}</span>
          <strong>${publisher.publisherName}</strong>
          <span class="text-muted"> - ${publisher.publisherCode}, ${publisher.publisherEmail ?? "no email"}</span>
        </div>
        <div class="d-flex gap-2">
          <button class="btn btn-sm btn-outline-primary edit-publisher-btn" data-id="${publisher.publisherId}">Edit</button>
          <button class="btn btn-sm btn-outline-danger delete-publisher-btn" data-id="${publisher.publisherId}">Delete</button>
        </div>
      </div>
    </div>
  `;
}

function attachPublisherHandlers() {
  document.querySelectorAll(".edit-publisher-btn").forEach(btn => {
    btn.addEventListener("click", () => startEditPublisher(btn.dataset.id));
  });

  document.querySelectorAll(".delete-publisher-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      if (!confirm("Delete this publisher?")) return;
      try {
        await deletePublisher(btn.dataset.id);
        loadPublishers();
        showCatalogBanner("Publisher deleted.");
      } catch (err) {
        // PublishersController returns 409 Conflict if the publisher still has books
        alert("Could not delete: " + err.message);
      }
    });
  });
}

function startEditPublisher(id) {
  const publisher = publishersData.find(p => String(p.publisherId) === String(id));
  const card = document.querySelector(`#publishersContainer [data-id="${id}"]`);

  card.innerHTML = `
    <div class="card-body">
      <span class="badge bg-secondary mb-2">Editing ID ${publisher.publisherId}</span>
      <span class="text-muted small ms-2">Code: ${publisher.publisherCode} (can't be changed)</span>
      <div class="row g-2 mt-1">
        <div class="col-md-3"><input type="text" class="form-control" id="editPublisherName-${id}" placeholder="Name" value="${publisher.publisherName ?? ""}"></div>
        <div class="col-md-3"><input type="text" class="form-control" id="editPublisherAddress-${id}" placeholder="Address" value="${publisher.publisherAddress ?? ""}"></div>
        <div class="col-md-2"><input type="text" class="form-control" id="editPublisherLandline-${id}" placeholder="Landline No" value="${publisher.publisherLandlineNo ?? ""}"></div>
        <div class="col-md-2"><input type="email" class="form-control" id="editPublisherEmail-${id}" placeholder="Email" value="${publisher.publisherEmail ?? ""}"></div>
        <div class="col-md-2 d-flex gap-2">
          <button class="btn btn-sm btn-success save-publisher-btn" data-id="${id}">Save</button>
          <button class="btn btn-sm btn-secondary cancel-publisher-btn" data-id="${id}">Cancel</button>
        </div>
      </div>
    </div>
  `;

  card.querySelector(".save-publisher-btn").addEventListener("click", async () => {
    const updated = {
      publisherName: document.getElementById(`editPublisherName-${id}`).value,
      publisherAddress: document.getElementById(`editPublisherAddress-${id}`).value,
      publisherLandlineNo: document.getElementById(`editPublisherLandline-${id}`).value,
      publisherEmail: document.getElementById(`editPublisherEmail-${id}`).value
    };

    try {
      await updatePublisher(id, updated);
      loadPublishers();
      showCatalogBanner(`"${updated.publisherName}" was updated.`);
    } catch (err) {
      alert("Could not update publisher: " + err.message);
    }
  });

  card.querySelector(".cancel-publisher-btn").addEventListener("click", loadPublishers);
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
      showCatalogBanner(`"${publisher.publisherName}" was added.`);
    } catch (err) {
      alert("Could not add publisher: " + err.message);
    }
  });
}

// ---- Categories ----

let categoriesData = [];

async function loadCategories() {
  const container = document.getElementById("categoriesContainer");
  try {
    categoriesData = await getCategories();
    container.innerHTML = categoriesData.length ? categoriesData.map(renderCategoryRow).join("") : '<p class="text-muted">No categories yet.</p>';
    attachCategoryHandlers();
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">${err.message}</div>`;
  }
}

function renderCategoryRow(category) {
  return `
    <div class="card mb-2" data-id="${category.categoryId}">
      <div class="card-body d-flex justify-content-between align-items-center flex-wrap gap-2">
        <div>
          <span class="badge bg-secondary">ID ${category.categoryId}</span>
          <strong>${category.categoryName}</strong>
          <span class="text-muted"> - ${category.categoryDescription ?? ""}</span>
        </div>
        <div class="d-flex gap-2">
          <button class="btn btn-sm btn-outline-primary edit-category-btn" data-id="${category.categoryId}">Edit</button>
          <button class="btn btn-sm btn-outline-danger delete-category-btn" data-id="${category.categoryId}">Delete</button>
        </div>
      </div>
    </div>
  `;
}

function attachCategoryHandlers() {
  document.querySelectorAll(".edit-category-btn").forEach(btn => {
    btn.addEventListener("click", () => startEditCategory(btn.dataset.id));
  });

  document.querySelectorAll(".delete-category-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      if (!confirm("Delete this category?")) return;
      try {
        await deleteCategory(btn.dataset.id);
        loadCategories();
        showCatalogBanner("Category deleted.");
      }
      catch (err) { alert("Could not delete: " + err.message); }
    });
  });
}

function startEditCategory(id) {
  const category = categoriesData.find(c => String(c.categoryId) === String(id));
  const card = document.querySelector(`#categoriesContainer [data-id="${id}"]`);

  card.innerHTML = `
    <div class="card-body">
      <span class="badge bg-secondary mb-2">Editing ID ${category.categoryId}</span>
      <div class="row g-2">
        <div class="col-md-4"><input type="text" class="form-control" id="editCategoryName-${id}" placeholder="Name" value="${category.categoryName ?? ""}"></div>
        <div class="col-md-6"><input type="text" class="form-control" id="editCategoryDescription-${id}" placeholder="Description" value="${category.categoryDescription ?? ""}"></div>
        <div class="col-md-2 d-flex gap-2">
          <button class="btn btn-sm btn-success save-category-btn" data-id="${id}">Save</button>
          <button class="btn btn-sm btn-secondary cancel-category-btn" data-id="${id}">Cancel</button>
        </div>
      </div>
    </div>
  `;

  card.querySelector(".save-category-btn").addEventListener("click", async () => {
    const updated = {
      categoryName: document.getElementById(`editCategoryName-${id}`).value,
      categoryDescription: document.getElementById(`editCategoryDescription-${id}`).value
    };

    try {
      await updateCategory(id, updated);
      loadCategories();
      showCatalogBanner(`"${updated.categoryName}" was updated.`);
    } catch (err) {
      alert("Could not update category: " + err.message);
    }
  });

  card.querySelector(".cancel-category-btn").addEventListener("click", loadCategories);
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
      showCatalogBanner(`"${category.categoryName}" was added.`);
    } catch (err) {
      alert("Could not add category: " + err.message);
    }
  });
}

// ---- Shelves ----

let shelvesData = [];

async function loadShelves() {
  const container = document.getElementById("shelvesContainer");
  try {
    shelvesData = await getShelves();
    container.innerHTML = shelvesData.length ? shelvesData.map(renderShelfRow).join("") : '<p class="text-muted">No shelves yet.</p>';
    attachShelfHandlers();
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">${err.message}</div>`;
  }
}

function renderShelfRow(shelf) {
  return `
    <div class="card mb-2" data-id="${shelf.shelfId}">
      <div class="card-body d-flex justify-content-between align-items-center flex-wrap gap-2">
        <div>
          <span class="badge bg-secondary">ID ${shelf.shelfId}</span>
          <strong>${shelf.shelfCode}</strong>
          <span class="text-muted"> - ${shelf.section}, Floor ${shelf.floorNumber}</span>
        </div>
        <div class="d-flex gap-2">
          <button class="btn btn-sm btn-outline-primary edit-shelf-btn" data-id="${shelf.shelfId}">Edit</button>
          <button class="btn btn-sm btn-outline-danger delete-shelf-btn" data-id="${shelf.shelfId}">Delete</button>
        </div>
      </div>
    </div>
  `;
}

function attachShelfHandlers() {
  document.querySelectorAll(".edit-shelf-btn").forEach(btn => {
    btn.addEventListener("click", () => startEditShelf(btn.dataset.id));
  });

  document.querySelectorAll(".delete-shelf-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      if (!confirm("Delete this shelf?")) return;
      try {
        await deleteShelf(btn.dataset.id);
        loadShelves();
        showCatalogBanner("Shelf deleted.");
      }
      catch (err) { alert("Could not delete: " + err.message); }
    });
  });
}

function startEditShelf(id) {
  const shelf = shelvesData.find(s => String(s.shelfId) === String(id));
  const card = document.querySelector(`#shelvesContainer [data-id="${id}"]`);

  card.innerHTML = `
    <div class="card-body">
      <span class="badge bg-secondary mb-2">Editing ID ${shelf.shelfId}</span>
      <div class="row g-2">
        <div class="col-md-4"><input type="text" class="form-control" id="editShelfCode-${id}" placeholder="Shelf code" value="${shelf.shelfCode ?? ""}"></div>
        <div class="col-md-4"><input type="text" class="form-control" id="editShelfSection-${id}" placeholder="Section" value="${shelf.section ?? ""}"></div>
        <div class="col-md-2"><input type="number" class="form-control" id="editShelfFloor-${id}" placeholder="Floor #" value="${shelf.floorNumber ?? ""}"></div>
        <div class="col-md-2 d-flex gap-2">
          <button class="btn btn-sm btn-success save-shelf-btn" data-id="${id}">Save</button>
          <button class="btn btn-sm btn-secondary cancel-shelf-btn" data-id="${id}">Cancel</button>
        </div>
      </div>
    </div>
  `;

  card.querySelector(".save-shelf-btn").addEventListener("click", async () => {
    const updated = {
      shelfCode: document.getElementById(`editShelfCode-${id}`).value,
      section: document.getElementById(`editShelfSection-${id}`).value,
      floorNumber: Number(document.getElementById(`editShelfFloor-${id}`).value)
    };

    try {
      await updateShelf(id, updated);
      loadShelves();
      showCatalogBanner(`"${updated.shelfCode}" was updated.`);
    } catch (err) {
      alert("Could not update shelf: " + err.message);
    }
  });

  card.querySelector(".cancel-shelf-btn").addEventListener("click", loadShelves);
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
      showCatalogBanner(`"${shelf.shelfCode}" was added.`);
    } catch (err) {
      alert("Could not add shelf: " + err.message);
    }
  });
}
