// ============================================================
// book-form.js — powers book-form.html
// Same page, same form, does double duty:
//   - no "?id=" in the URL  -> Create mode (POST)
//   - "?id=123" in the URL  -> Edit mode (GET to pre-fill, then PUT)
// Staff only - gate access to this page the same way dashboard.html does.
//
// Note: publisherId/categoryId are plain number inputs here for simplicity.
// A <select> populated from Publisher/Category GetAll endpoints would be a
// nicer upgrade later, but needs those two controllers' list endpoints.
// authorIds is a simple comma-separated input (e.g. "1,2") parsed into an
// array of numbers - BookDto requires a List<int>, not a single value.
// ============================================================
 
document.addEventListener("DOMContentLoaded", initForm);
 
function getIdFromQueryString() {
  const params = new URLSearchParams(window.location.search);
  return params.get("id");
}
 
async function initForm() {
  const id = getIdFromQueryString();
  const form = document.getElementById("bookForm");
  const heading = document.getElementById("formHeading");
 
  if (id) {
    heading.textContent = "Edit Book";
 
    const book = await getBook(id);
    document.getElementById("isbn").value = book.isbn;
    document.getElementById("bookTitle").value = book.bookTitle;
    document.getElementById("bookEdition").value = book.bookEdition ?? "";
    document.getElementById("bookLanguage").value = book.bookLanguage;
    document.getElementById("year").value = book.year ?? "";
    document.getElementById("publisherId").value = book.publisherId ?? "";
    document.getElementById("categoryId").value = book.categoryId ?? "";
    document.getElementById("authorIds").value = (book.authors || []).map(a => a.authorId).join(",");
  } else {
    heading.textContent = "Add New Book";
  }
 
  form.addEventListener("submit", async (event) => {
    event.preventDefault(); // stop the browser's default full-page form submit
 
    const authorIdsText = document.getElementById("authorIds").value;
    const authorIds = authorIdsText
      .split(",")
      .map(s => s.trim())
      .filter(s => s.length > 0)
      .map(Number);
 
    const book = {
      isbn: document.getElementById("isbn").value,
      bookTitle: document.getElementById("bookTitle").value,
      bookEdition: parseInt(document.getElementById("bookEdition").value, 10) || null,
      bookLanguage: document.getElementById("bookLanguage").value,
      year: parseInt(document.getElementById("year").value, 10) || null,
      publisherId: parseInt(document.getElementById("publisherId").value, 10) || null,
      categoryId: parseInt(document.getElementById("categoryId").value, 10) || null,
      authorIds: authorIds
    };
 
    try {
      if (id) {
        await updateBook(id, book);
        window.location.href = `book-details.html?id=${id}`;
      } else {
        const created = await createBook(book);
        window.location.href = `book-details.html?id=${created.bookId}`;
      }
    } catch (err) {
      alert("Something went wrong: " + err.message);
    }
  });
}