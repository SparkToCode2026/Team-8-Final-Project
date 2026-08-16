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

  return `
    <div class="card mb-2" data-event-id="${event.eventId}">
      <div class="card-body d-flex justify-content-between align-items-center flex-wrap gap-2">
        <div>
          <strong>${event.eventName}</strong>
          <span class="text-muted"> - ${new Date(event.eventDate).toLocaleString()}, ${event.eventLocation}</span>
        </div>
        <div class="d-flex gap-2">
          <select class="form-select form-select-sm event-status-select">
            ${statuses.map(s => `<option value="${s}" ${s === event.status ? "selected" : ""}>${s}</option>`).join("")}
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
      try { await updateEventStatus(card.dataset.eventId, newStatus); loadEvents(); }
      catch (err) { alert("Could not update event: " + err.message); }
    });
  });

  document.querySelectorAll(".delete-event-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      const card = btn.closest("[data-event-id]");
      if (!confirm("Delete this event?")) return;
      try { await deleteEvent(card.dataset.eventId); loadEvents(); }
      catch (err) { alert("Could not delete event: " + err.message); }
    });
  });
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
    } catch (err) {
      alert("Could not add event: " + err.message);
    }
  });
}
