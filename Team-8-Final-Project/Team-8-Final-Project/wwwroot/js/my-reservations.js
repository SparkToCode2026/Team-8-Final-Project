// ============================================================
// my-reservations.js — powers my-reservations.html
//
// Same read-only situation as my-loans.js: RemoveReservation is
// Librarian/Admin-only server-side, so a member can't cancel their own
// reservation here yet - only view it.
// ============================================================

document.addEventListener("DOMContentLoaded", loadMyReservations);

async function loadMyReservations() {
  const container = document.getElementById("reservationsContainer");
  const userId = getUserId();

  try {
    // GetReservationsByUser returns bare rows with no book attached, so
    // this fetches every book once and matches them up client-side below.
    const [reservations, books] = await Promise.all([
      getReservationsByUser(userId),
      getBooks()
    ]);

    if (reservations.length === 0) {
      container.innerHTML = '<p class="text-muted">You have no reservations yet.</p>';
      return;
    }

    // bookId -> book, for quick lookup below
    const bookLookup = {};
    books.forEach(book => { bookLookup[book.bookId] = book; });

    container.innerHTML = reservations.map(r => renderReservationCard(r, bookLookup[r.bookId])).join("");
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">Could not load reservations: ${err.message}</div>`;
  }
}

function renderReservationCard(reservation, book) {
  const title = book?.bookTitle ?? "Unknown book";

  // Maps each ReservationStatus enum value to a Bootstrap badge color
  const statusClass = {
    Active: "bg-primary",
    Cancelled: "bg-secondary",
    Completed: "bg-success"
  }[reservation.status] ?? "bg-secondary";

  return `
    <div class="card mb-3">
      <div class="card-body d-flex justify-content-between align-items-center">
        <div>
          <h5 class="card-title mb-1">${title}</h5>
          <p class="card-text text-muted mb-0">
            Reserved on: ${new Date(reservation.reservationDate).toLocaleDateString()}
          </p>
        </div>
        <span class="badge ${statusClass}">${reservation.status}</span>
      </div>
    </div>
  `;
}
