// ============================================================
// events.js — powers staff/events.html
// Full CRUD, unlike the read-only member/events.js.
// ============================================================

document.addEventListener("DOMContentLoaded", () => {
  loadEvents();
  setupAddEventForm();
});

async function loadEvents() {
  const container = document.getElementById("eventsContainer");
  try {
    const events = await getAllEvents();
    container.innerHTML = events.length ? events.map(renderEventRow).join("") : '<p class="text-muted">No events yet.</p>';
    attachEventHandlers();
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">${err.message}</div>`;
  }
}

function renderEventRow(event) {
  const statuses = ["Upcoming", "Ongoing", "Completed", "Cancelled"];

  // Same fix as admin/users.js's role dropdown: the API can return status as
  // a raw enum number (0/1/2/3) instead of its string name, which broke the
  // s === event.status check below and made every row default to showing
  // the first option ("Upcoming") regardless of the real status. Also
  // guarding eventId against alternate casings, same reasoning as userId.
  const id = event.eventId ?? event.eventID ?? event.EventId ?? event.EventID ?? event.id ?? event.Id ?? event.ID;
  const currentStatus = typeof event.status === "number" ? statuses[event.status] : event.status;

  return `
    <div class="card mb-2" data-event-id="${id}">
      <div class="card-body d-flex justify-content-between align-items-center flex-wrap gap-2">
        <div>
          <strong>${event.eventName}</strong>
          <span class="text-muted"> - ${new Date(event.eventDate).toLocaleString()}, ${event.eventLocation}</span>
        </div>
        <div class="d-flex gap-2">
          <select class="form-select form-select-sm event-status-select">
            ${statuses.map(s => `<option value="${s}" ${s === currentStatus ? "selected" : ""}>${s}</option>`).join("")}
          </select>
          <button class="btn btn-sm btn-outline-primary update-event-btn">Update</button>
          <button class="btn btn-sm btn-outline-danger delete-event-btn">Delete</button>
        </div>
      </div>
    </div>
  `;
}

function attachEventHandlers() {
  document.querySelectorAll(".update-event-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      const card = btn.closest("[data-event-id]");
      const newStatus = card.querySelector(".event-status-select").value;
      const name = card.querySelector("strong").textContent;
      try {
        await updateEventStatus(card.dataset.eventId, newStatus);
        loadEvents();
        showEventsBanner(`"${name}" was updated to ${newStatus}.`);
      }
      catch (err) { alert("Could not update event: " + err.message); }
    });
  });

  document.querySelectorAll(".delete-event-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      const card = btn.closest("[data-event-id]");
      const name = card.querySelector("strong").textContent;
      if (!confirm("Delete this event?")) return;
      try {
        await deleteEvent(card.dataset.eventId);
        loadEvents();
        showEventsBanner(`"${name}" was deleted.`);
      }
      catch (err) { alert("Could not delete event: " + err.message); }
    });
  });
}

// Success confirmations, same pattern as admin/users.js's showUsersBanner -
// these actions used to succeed silently with nothing on screen to confirm
// it. Fades out on its own after a few seconds.
function showEventsBanner(message) {
  const banner = document.getElementById("eventsBanner");
  banner.innerHTML = `<div class="alert alert-success">${message}</div>`;
  setTimeout(() => { banner.innerHTML = ""; }, 4000);
}

function setupAddEventForm() {
  document.getElementById("addEventForm").addEventListener("submit", async (event) => {
    event.preventDefault();

    const newEvent = {
      eventName: document.getElementById("eventName").value,
      eventDate: document.getElementById("eventDate").value,
      eventLocation: document.getElementById("eventLocation").value,
      eventMaxCap: Number(document.getElementById("eventMaxCap").value),
      eventDescription: document.getElementById("eventDescription").value
    };

    try {
      await addEvent(newEvent);
      event.target.reset();
      loadEvents();
      showEventsBanner(`"${newEvent.eventName}" was added.`);
    } catch (err) {
      alert("Could not add event: " + err.message);
    }
  });
}
