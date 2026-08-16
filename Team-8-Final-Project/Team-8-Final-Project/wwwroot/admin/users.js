// ============================================================
// users.js — powers admin/users.html
// ============================================================

document.addEventListener("DOMContentLoaded", loadUsers);

async function loadUsers() {
  const container = document.getElementById("usersContainer");
  try {
    const users = await getAllUsers();
    container.innerHTML = users.length ? users.map(renderUserRow).join("") : '<p class="text-muted">No users found.</p>';
    attachUserHandlers();
  } catch (err) {
    container.innerHTML = `<div class="alert alert-danger">${err.message}</div>`;
  }
}

function renderUserRow(user) {
  const roles = ["Member", "Librarian", "Admin"];

  return `
    <div class="card mb-2" data-user-id="${user.userId}">
      <div class="card-body d-flex justify-content-between align-items-center flex-wrap gap-2">
        <div>
          <strong>${user.firstName} ${user.lastName}</strong>
          <span class="text-muted"> - ${user.userEmail}</span>
        </div>
        <div class="d-flex gap-2">
          <select class="form-select form-select-sm role-select">
            ${roles.map(r => `<option value="${r}" ${r === user.role ? "selected" : ""}>${r}</option>`).join("")}
          </select>
          <button class="btn btn-sm btn-outline-primary update-role-btn">Update role</button>
          <button class="btn btn-sm btn-outline-danger delete-user-btn">Remove</button>
        </div>
      </div>
    </div>
  `;
}

function attachUserHandlers() {
  document.querySelectorAll(".update-role-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      const card = btn.closest("[data-user-id]");
      const newRole = card.querySelector(".role-select").value;
      try { await changeUserRole(card.dataset.userId, newRole); loadUsers(); }
      catch (err) { alert("Could not update role: " + err.message); }
    });
  });

  document.querySelectorAll(".delete-user-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      const card = btn.closest("[data-user-id]");
      if (!confirm("Remove this user account? This can't be undone.")) return;
      try { await removeUser(card.dataset.userId); loadUsers(); }
      catch (err) { alert("Could not remove user: " + err.message); }
    });
  });
}
