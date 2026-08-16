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

  // Most endpoints respond with JSON, but quite a few (Register, ForgotPassword,
  // ResetPassword, and nearly every Delete/Remove endpoint) respond with
  // Ok("some plain string") instead, which ASP.NET Core sends back as
  // text/plain, not application/json. Calling .json() on that crashes with
  // "Unexpected token" even though the request actually succeeded - so check
  // the Content-Type before deciding how to read the body.
  const contentType = response.headers.get("content-type") || "";
  const isJson = contentType.includes("application/json");

  if (!response.ok) {
    const text = isJson ? JSON.stringify(await response.json()) : await response.text();
    throw new Error(text || `Request failed: ${response.status}`);
  }

  if (response.status === 204) return null; // e.g. successful PUT/DELETE, no body
  return isJson ? response.json() : response.text();
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

// ---- Review endpoints ---- (confirmed against the real ReviewController.cs)
function addReview(review) {
  // review must include: rating, comment, bookId - userId is set server-side from the JWT, not sent
  return apiFetch("/Review/AddReview", { method: "POST", body: JSON.stringify(review) });
}

function updateReview(id, review) {
  // review must include: rating, comment
  return apiFetch(`/Review/UpdateReview?id=${id}`, { method: "PUT", body: JSON.stringify(review) });
}

function updateReviewComment(id, comment) {
  return apiFetch(`/Review/UpdateReviewComment?id=${id}&comment=${encodeURIComponent(comment)}`, { method: "PATCH" });
}

function deleteReview(id) {
  return apiFetch(`/Review/DeleteReview?id=${id}`, { method: "DELETE" });
}

function getReviewsByBook(bookId) {
  return apiFetch(`/Review/GetReviewsByBook?bookId=${bookId}`);
}

function getAverageRating(bookId) {
  return apiFetch(`/Review/GetAverageRating?bookId=${bookId}`);
}

// ---- Event endpoints ---- (confirmed against the real EventController.cs)
function getEventsSortedByDate() {
  return apiFetch("/Event/GetEventsSortedByDate");
}

// ---- Category endpoints ---- (confirmed against the real CategoryController.cs)
function addCategory(category) {
  return apiFetch("/Category/AddCategory", { method: "POST", body: JSON.stringify(category) });
}
function updateCategory(id, category) {
  return apiFetch(`/Category/UpdateCategory?id=${id}`, { method: "PUT", body: JSON.stringify(category) });
}
function deleteCategory(id) {
  return apiFetch(`/Category/DeleteCategory?id=${id}`, { method: "DELETE" });
}
function getCategories() {
  return apiFetch("/Category/GetAllCategories");
}
function getBookCountByCategory() {
  return apiFetch("/Category/GetBookCountByCategory");
}

// ---- Author (staff CRUD) endpoints ---- (confirmed against the real AuthorsController.cs)
function addAuthor(author) {
  return apiFetch("/Author/AddAuthor", { method: "POST", body: JSON.stringify(author) });
}
function updateAuthor(id, author) {
  return apiFetch(`/Author/UpdateAuthor?id=${id}`, { method: "PUT", body: JSON.stringify(author) });
}
function deleteAuthor(id) {
  return apiFetch(`/Author/DeleteAuthor?id=${id}`, { method: "DELETE" });
}
function getAuthors() {
  return apiFetch("/Author/GetAllAuthors");
}
function getAuthorStats() {
  return apiFetch("/Author/GetAuthorStats");
}

// ---- BookCopy (staff CRUD) endpoints ---- (confirmed against the real BookCopyController.cs)
function addBookCopy(copy) {
  // copy must include: barcode, condition, availabilityStatus, copyPrice, bookId, shelfId
  return apiFetch("/BookCopy/AddBookCopy", { method: "POST", body: JSON.stringify(copy) });
}
function updateBookCopy(id, copy) {
  // copy must include: barcode, condition, availabilityStatus, copyPrice, shelfId (no bookId on update)
  return apiFetch(`/BookCopy/UpdateBookCopy?id=${id}`, { method: "PUT", body: JSON.stringify(copy) });
}
function updateBookCopyStatus(id, condition, availabilityStatus) {
  // both are optional - pass null/undefined for whichever one you're not changing
  const params = new URLSearchParams({ id });
  if (condition) params.append("condition", condition);
  if (availabilityStatus) params.append("availabilityStatus", availabilityStatus);
  return apiFetch(`/BookCopy/UpdateBookCopyStatus?${params.toString()}`, { method: "PATCH" });
}
function deleteBookCopy(id) {
  return apiFetch(`/BookCopy/DeleteBookCopy?id=${id}`, { method: "DELETE" });
}
function getAllBookCopies() {
  return apiFetch("/BookCopy/GetAllBookCopies");
}
function getCopyCountByStatus() {
  return apiFetch("/BookCopy/GetCopyCountByStatus");
}

// ---- Publisher endpoints ---- (confirmed against the real PublishersController.cs)
// Note: like Fine and Shelf, this one lives under /api/Publishers and uses
// route-param ids ({id} in the URL), not the ?id= query string most other
// controllers use.
function addPublisher(publisher) {
  // publisher must include: publisherCode, publisherName, publisherAddress, publisherLandlineNo, publisherEmail
  return apiFetch("/api/Publishers", { method: "POST", body: JSON.stringify(publisher) });
}
function updatePublisher(id, publisher) {
  // publisher must include: publisherName, publisherAddress, publisherLandlineNo, publisherEmail (no code on update)
  return apiFetch(`/api/Publishers/${id}`, { method: "PUT", body: JSON.stringify(publisher) });
}
function deletePublisher(id) {
  // fails with 409 Conflict if the publisher still has books - reassign them first
  return apiFetch(`/api/Publishers/${id}`, { method: "DELETE" });
}
function reassignBooks(oldPublisherId, newPublisherId) {
  return apiFetch(`/api/Publishers/${oldPublisherId}/reassign-books/${newPublisherId}`, { method: "PUT" });
}
function getPublishers() {
  return apiFetch("/api/Publishers");
}

// ---- Shelf endpoints ---- (confirmed against the real ShelfController.cs)
// Note: like Fine, this one lives under /api/Shelf, and it uses route-param ids
// ({id} in the URL) instead of the ?id= query string style most other controllers use.
function addShelf(shelf) {
  return apiFetch("/api/Shelf", { method: "POST", body: JSON.stringify(shelf) });
}
function updateShelf(id, shelf) {
  return apiFetch(`/api/Shelf/${id}`, { method: "PUT", body: JSON.stringify(shelf) });
}
function deleteShelf(id) {
  return apiFetch(`/api/Shelf/${id}`, { method: "DELETE" });
}
function getShelves() {
  return apiFetch("/api/Shelf");
}

// ---- Event (staff CRUD) endpoints ---- (confirmed against the real EventController.cs)
function addEvent(event) {
  return apiFetch("/Event/AddEvent", { method: "POST", body: JSON.stringify(event) });
}
function updateEvent(id, event) {
  return apiFetch(`/Event/UpdateEvent?id=${id}`, { method: "PUT", body: JSON.stringify(event) });
}
function updateEventStatus(id, newStatus) {
  return apiFetch(`/Event/UpdateEventStatus?id=${id}&newStatus=${newStatus}`, { method: "PATCH" });
}
function deleteEvent(id) {
  return apiFetch(`/Event/DeleteEvent?id=${id}`, { method: "DELETE" });
}
function getAllEvents() {
  // includes registered Users per event, unlike getEventsSortedByDate
  return apiFetch("/Event/GetAllEvents");
}

// ---- Review (staff extras) ---- (confirmed against the real ReviewController.cs)
function deleteReviewAsStaff(id) {
  return deleteReview(id); // same endpoint - Librarian/Admin are allowed same as the review's own author
}
function filterHighRatingReviews(bookId) {
  return apiFetch(`/Review/FilterHighRatingReviews?bookId=${bookId}`);
}

// ---- User (admin) endpoints ---- (confirmed against the real UsersController.cs)
function updateUser(id, user) {
  return apiFetch(`/User/UpdateUser?id=${id}`, { method: "PUT", body: JSON.stringify(user) });
}
function changeUserRole(id, newRole) {
  // newRole is the UserRole enum name as a string: "Member", "Librarian", "Admin"
  return apiFetch(`/User/ChangeUserRole?id=${id}&newRole=${newRole}`, { method: "PATCH" });
}
function removeUser(id) {
  return apiFetch(`/User/RemoveUser?id=${id}`, { method: "DELETE" });
}
function getAllUsers() {
  return apiFetch("/User/FetchAllUsers");
}
function getUsersSummary() {
  return apiFetch("/User/GetUsersSummary");
}

// ---- Auth (register / password reset) endpoints ---- (confirmed against the real AuthController.cs)
function register(user) {
  // user must include: firstName, lastName, userEmail, userPhoneNo, dob, password
  return apiFetch("/Auth/Register", { method: "POST", body: JSON.stringify(user) });
}
function forgotPassword(userEmail) {
  return apiFetch("/Auth/ForgotPassword", { method: "POST", body: JSON.stringify({ userEmail }) });
}
function resetPassword(userEmail, resetToken, newPassword) {
  return apiFetch("/Auth/ResetPassword", {
    method: "POST",
    body: JSON.stringify({ userEmail, resetToken, newPassword })
  });
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
