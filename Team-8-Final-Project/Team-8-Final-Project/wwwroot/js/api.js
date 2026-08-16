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
 
// ---- Loan endpoints ---- (confirmed against the real LoanController.cs)
function createLoan(loan) {
  // loan must include: loanDueDate, bookCopyId, userID
  return apiFetch("/Loan/AddLoan", { method: "POST", body: JSON.stringify(loan) });
}
 
function getLoan(id) {
  return apiFetch(`/Loan/GetLoan?id=${id}`);
}
 
function getAllLoans() {
  return apiFetch("/Loan/GetAllLoans");
}
 
function updateLoan(id, loan) {
  // loan must include: loanStartDate, loanDueDate, bookCopyId, userID
  return apiFetch(`/Loan/UpdateLoan?id=${id}`, { method: "PUT", body: JSON.stringify(loan) });
}
 
function updateLoanStatus(id, newStatus) {
  // newStatus is the LoanStatus enum name as a string, e.g. "Active", "Returned", "Overdue"
  return apiFetch(`/Loan/UpdateLoanStatus?id=${id}&newStatus=${newStatus}`, { method: "PATCH" });
}
 
function deleteLoan(id) {
  return apiFetch(`/Loan/RemoveLoan?id=${id}`, { method: "DELETE" });
}
 
function getLoansByUser(userId) {
  // note: backend doesn't scope this to the logged-in user server-side,
  // so always call it with the id decoded from the current JWT, never a typed-in value
  return apiFetch(`/Loan/GetLoansByUser?userId=${userId}`);
}
 
function getLoansByBookCopy(bookCopyId) {
  return apiFetch(`/Loan/GetLoansByBookCopy?bookCopyId=${bookCopyId}`);
}
 
function getLoansSortedByDueDate() {
  return apiFetch("/Loan/GetLoansSortedByDueDate");
}
 
// ---- Reservation endpoints ---- (confirmed against the real ReservationController.cs)
function createReservation(reservation) {
  // reservation must include: reservationDate, bookId, userId
  return apiFetch("/Reservation/AddReservation", { method: "POST", body: JSON.stringify(reservation) });
}
 
function getReservationById(id) {
  return apiFetch(`/Reservation/GetReservationById?id=${id}`);
}
 
function getAllReservations() {
  return apiFetch("/Reservation/GetAllReservation");
}
 
function updateReservation(id, reservation) {
  // reservation must include: reservationDate, bookId, userId
  return apiFetch(`/Reservation/UpdateReservation?id=${id}`, { method: "PUT", body: JSON.stringify(reservation) });
}
 
function updateReservationStatus(id, newStatus) {
  // newStatus is the ReservationStatus enum name as a string, e.g. "Active", "Fulfilled", "Cancelled"
  return apiFetch(`/Reservation/UpdateReservationStatus?reservationId=${id}`, {
    method: "PATCH",
    body: JSON.stringify({ newStatus })
  });
}
 
function deleteReservation(id) {
  return apiFetch(`/Reservation/RemoveReservation?id=${id}`, { method: "DELETE" });
}
 
function getReservationsByUser(userId) {
  // same note as getLoansByUser - not server-scoped, only ever call with the current user's id
  return apiFetch(`/Reservation/GetReservationsByUser?userId=${userId}`);
}
 
function getReservationsSortedByDate() {
  return apiFetch("/Reservation/GetReservationsSortedByDate");
}
 
// ---- Fine endpoints ---- (confirmed against the real FineController.cs)
// Note: this one lives under /api/Fine, not /Fine like every other controller -
// it's the only route in the project with that prefix.
function createFine(fine) {
  // fine is the raw entity, not a DTO - must include: loanId, fineAmount, fineIssueDate
  return apiFetch("/api/Fine", { method: "POST", body: JSON.stringify(fine) });
}
 
function updateFine(id, fine) {
  return apiFetch(`/api/Fine/${id}`, { method: "PUT", body: JSON.stringify(fine) });
}
 
function updateFineStatus(id, status) {
  // status is the FinePaymentStatus enum name as a string, e.g. "Paid", "Unpaid"
  return apiFetch(`/api/Fine/${id}/status?status=${status}`, { method: "PATCH" });
}
 
function deleteFine(id) {
  return apiFetch(`/api/Fine/${id}`, { method: "DELETE" });
}
 
function getAllFines() {
  return apiFetch("/api/Fine");
}
 
function getMyFines() {
  return apiFetch("/api/Fine/my-fines");
}
 
function getUnpaidFines() {
  return apiFetch("/api/Fine/unpaid");
}
 
function getTotalUnpaidFines() {
  return apiFetch("/api/Fine/total-unpaid");
}
 
// Reads the user id claim out of the JWT payload - use this (never a typed-in value)
// whenever a "my loans / my reservations" style page needs the current user's id,
// since GetLoansByUser/GetReservationsByUser aren't scoped server-side.
function getUserId() {
  const token = localStorage.getItem("token");
  if (!token) return null;
  const payload = JSON.parse(atob(token.split(".")[1]));
  return payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
}
 
// Reads the role claim out of the JWT payload so pages can hide staff-only buttons.
function getUserRole() {
  const token = localStorage.getItem("token");
  if (!token) return null;
  const payload = JSON.parse(atob(token.split(".")[1]));
  return payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
}