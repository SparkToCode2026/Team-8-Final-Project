// ============================================================
// users.js — powers admin/users.html
// ============================================================

const USERS_PAGE_SIZE = 20;

let allUsers = [];
let usersRoleFilter = "All";
let usersVisibleCount = USERS_PAGE_SIZE;

document.addEventListener("DOMContentLoaded", () => {
  document.querySelectorAll("#roleFilterGroup button").forEach(btn => {
    btn.addEventListener("click", () => {
      document.querySelectorAll("#roleFilterGroup button").forEach(b => b.classList.remove("active"));
      btn.classList.add("active");
      usersRoleFilter = btn.dataset.role;
      usersVisibleCount = USERS_PAGE_SIZE;
      renderUsersTable();
    });
  });

  document.getElementById("loadMoreUsersBtn").addEventListener("click", () => {
    usersVisibleCount += USERS_PAGE_SIZE;
    renderUsersTable();
  });

  loadUsers();
});

async function loadUsers() {
  try {
    allUsers = await getAllUsers();
  } catch (err) {
    document.getElementById("usersTableBody").innerHTML = `<tr><td colspan="4"><div class="alert alert-danger mb-0">${err.message}</div></td></tr>`;
    return;
  }
  renderUsersTable();
}

function roleOf(user) {
  const roles = ["Member", "Librarian", "Admin"];
  // Same fallback as the id lookup below - role has come back as either the
  // string name or the raw enum number (0/1/2) depending on the endpoint.
  return typeof user.role === "number" ? roles[user.role] : user.role;
}

function renderUsersTable() {
  const tbody = document.getElementById("usersTableBody");
  const emptyMessage = document.getElementById("usersEmptyMessage");
  const loadMoreBtn = document.getElementById("loadMoreUsersBtn");
  const countsBox = document.getElementById("usersCounts");

  const counts = { Member: 0, Librarian: 0, Admin: 0 };
  allUsers.forEach(u => {
    const role = roleOf(u);
    if (counts[role] !== undefined) counts[role]++;
  });
  countsBox.innerHTML = `<strong>${allUsers.length} total</strong> - ${counts.Member} members, ${counts.Librarian} librarians, ${counts.Admin} admins`;

  const filtered = usersRoleFilter === "All" ? allUsers : allUsers.filter(u => roleOf(u) === usersRoleFilter);
  const visible = filtered.slice(0, usersVisibleCount);

  if (visible.length === 0) {
    tbody.innerHTML = "";
    emptyMessage.classList.remove("d-none");
  } else {
    emptyMessage.classList.add("d-none");
    tbody.innerHTML = visible.map(renderUserRow).join("");
  }

  loadMoreBtn.classList.toggle("d-none", filtered.length <= visible.length);

  attachUserHandlers();
}

function renderUserRow(user) {
  const roles = ["Member", "Librarian", "Admin"];

  // The API's field names for these two turned out not to match the rest of
  // the app's usual camelCase convention - id came back undefined under
  // "userId", and role came back as a raw number (0/1/2) instead of a
  // string, so the old r === user.role check never matched anything and
  // every row silently fell back to showing "Member". These fallbacks
  // handle either shape without needing the backend changed.
  const id = user.userId ?? user.userID ?? user.UserId ?? user.UserID ?? user.id ?? user.Id ?? user.ID;
  const currentRole = roleOf(user);

  return `
    <tr data-user-id="${id}">
      <td>${user.firstName} ${user.lastName}</td>
      <td class="text-muted">${user.userEmail}</td>
      <td>
        <select class="form-select form-select-sm role-select">
          ${roles.map(r => `<option value="${r}" ${r === currentRole ? "selected" : ""}>${r}</option>`).join("")}
        </select>
      </td>
      <td class="text-end">
        <button class="btn btn-sm btn-outline-primary update-role-btn">Update role</button>
        <button class="btn btn-sm btn-outline-danger delete-user-btn">Remove</button>
      </td>
    </tr>
  `;
}

function attachUserHandlers() {
  document.querySelectorAll(".update-role-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      const row = btn.closest("[data-user-id]");
      const newRole = row.querySelector(".role-select").value;
      const name = row.querySelector("td").textContent;
      try {
        await changeUserRole(row.dataset.userId, newRole);
        await loadUsers();
        showUsersBanner(`${name}'s role was updated to ${newRole}.`);
      }
      catch (err) { alert("Could not update role: " + err.message); }
    });
  });

  document.querySelectorAll(".delete-user-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      const row = btn.closest("[data-user-id]");
      const name = row.querySelector("td").textContent;
      if (!confirm("Remove this user account? This can't be undone.")) return;
      try {
        await removeUser(row.dataset.userId);
        await loadUsers();
        showUsersBanner(`${name}'s account was removed.`);
      }
      catch (err) { alert("Could not remove user: " + err.message); }
    });
  });
}

// Success confirmations for actions that used to leave the admin guessing
// whether anything happened. Fades out on its own so it doesn't need a
// dismiss button, but loadUsers() re-rendering the table body won't touch
// this since it's a separate element outside that table.
function showUsersBanner(message) {
  const banner = document.getElementById("usersBanner");
  banner.innerHTML = `<div class="alert alert-success">${message}</div>`;
  setTimeout(() => { banner.innerHTML = ""; }, 4000);
}
