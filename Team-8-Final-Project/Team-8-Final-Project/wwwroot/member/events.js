// ============================================================
// events.js — powers events.html
//
// Read-only listing. EventController has no "register for this event"
// endpoint yet (Event.Users just tracks who's registered, but nothing
// lets a member add themselves to it) - so there's nothing clickable
// here, just information.
// ============================================================

document.addEventListener("DOMContentLoaded", loadEvents);

async function loadEvents() {
  const container = document.getElementById("eventsContainer");

  try {
    const events = await getEventsSortedByDate();

    if (events.length === 0) {
      container.innerHTML = '<p class="text-muted">No events scheduled right now.</p>';
      return;
    }

    container.innerHTML = events.map(renderEventCard).join("");
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">Could not load events: ${err.message}</div>`;
  }
}

function renderEventCard(event) {
  // Maps each EventStatus enum value to a Bootstrap badge color
  const statusClass = {
    Upcoming: "bg-primary",
    Ongoing: "bg-success",
    Completed: "bg-secondary",
    Cancelled: "bg-danger"
  }[event.status] ?? "bg-secondary";

  return `
    <div class="col-12 col-md-6 col-lg-4 mb-4">
      <div class="card h-100">
        <div class="card-body d-flex flex-column">
          <div class="d-flex justify-content-between align-items-start mb-2">
            <h5 class="card-title mb-0">${event.eventName}</h5>
            <span class="badge ${statusClass}">${event.status}</span>
          </div>
          <p class="card-text text-muted mb-1">${new Date(event.eventDate).toLocaleString()}</p>
          <p class="card-text text-muted mb-2">${event.eventLocation}</p>
          <p class="card-text">${event.eventDescription ?? ""}</p>
        </div>
      </div>
    </div>
  `;
}
