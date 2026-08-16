// ============================================================
// api.js
// Every fetch call to the backend goes through apiFetch(), so the
// JWT gets attached automatically instead of repeating it everywhere.
// ============================================================
 
const API_BASE_URL = "http://localhost:5240"; // must match whatever "Now listening on" prints in your terminal
 
async function apiFetch(path, options = {}) {
  const token = localStorage.getItem("token");
  const headers = {
    "Content-Type": "application/json",
    ...(token ? { "Authorization": `Bearer ${token}` } : {}),
    ...options.headers
  };
 
  const response = await fetch(`${API_BASE_URL}${path}`, { ...options, headers });
 
  if (response.status === 401) {
    // Token missing/expired - send the user back to log in
    localStorage.removeItem("token");
    window.location.href = "/login.html";
    return;
  }
 
  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || `Request failed: ${response.status}`);
  }
 
  if (response.status === 204) return null; // e.g. successful PUT/DELETE, no body
  return response.json();
}
 
// ---- Book endpoints ---- (confirmed against the real BookController.cs)
function getBooks() {
  return apiFetch("/Book/GetAllBooks");
}
 
function getBook(id) {
  return apiFetch(`/Book/GetBookById?id=${id}`);
}
 
function createBook(book) {
  // book must include: isbn, bookTitle, bookEdition, bookLanguage, year,
  // publisherId, categoryId, authorIds (array) - all required by BookDto
  return apiFetch("/Book/AddBook", { method: "POST", body: JSON.stringify(book) });
}
 
function updateBook(id, book) {
  // note: id is part of the URL here, not a query string, because
  // UpdateBook is [HttpPut("UpdateBook/{id}")]
  return apiFetch(`/Book/UpdateBook/${id}`, { method: "PUT", body: JSON.stringify(book) });
}
 
function deleteBook(id) {
  return apiFetch(`/Book/DeleteBook?id=${id}`, { method: "DELETE" });
}
 
// Reads the role claim out of the JWT payload so pages can hide staff-only buttons.
function getUserRole() {
  const token = localStorage.getItem("token");
  if (!token) return null;
  const payload = JSON.parse(atob(token.split(".")[1]));
  return payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
}