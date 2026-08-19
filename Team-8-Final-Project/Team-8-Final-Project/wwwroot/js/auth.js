// ============================================================
// auth.js — handles the login form on login.html
// (register.html would need a similar handler for POST /Auth/Register)
// ============================================================

document.addEventListener("DOMContentLoaded", () => {
  showPasswordResetBannerIfNeeded();

  const form = document.getElementById("loginForm");
  if (!form) return; // this script is safe to include on pages without a login form

  form.addEventListener("submit", async (event) => {
    event.preventDefault(); // stop the browser's default full-page form submit

    const userEmail = document.getElementById("userEmail").value;
    const password = document.getElementById("password").value;

    try {
      const response = await fetch(`${API_BASE_URL}/Auth/Login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ userEmail, password })
      });

      if (!response.ok) {
        throw new Error("Login failed - check your email and password.");
      }

      const data = await response.json();
      localStorage.setItem("token", data.token);

      // Send each role to its own dashboard. Admin and Librarian share the
      // same staff dashboard - Admin just sees an extra "Manage Users" card
      // there, linking into admin/users.html (see staff/dashboard.html).
      const role = getUserRole();
      if (role === "Admin" || role === "Librarian") {
        window.location.href = "staff/dashboard.html";
      } else {
        window.location.href = "member/dashboard.html";
      }
    } catch (err) {
      alert(err.message);
    }
  });
});

// reset-password.js redirects here (instead of an alert() popup) right after
// a successful password reset, adding ?passwordReset=1 to the URL - shows a
// one-time confirmation banner, then cleans the URL so a refresh won't
// re-show it. Same pattern as showReservationBannerIfNeeded() in
// my-reservations.js.
function showPasswordResetBannerIfNeeded() {
  const params = new URLSearchParams(window.location.search);
  if (params.get("passwordReset") !== "1") return;

  const banner = document.getElementById("loginBanner");
  banner.innerHTML = '<div class="alert alert-success">Password reset! You can log in with your new password now.</div>';

  history.replaceState(null, "", window.location.pathname);
}
