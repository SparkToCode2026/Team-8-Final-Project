// ============================================================
// reset-password.js — handles the form on reset-password.html
// Reached via the link emailed by AuthController's ForgotPassword, which
// appends ?email=...&token=... to the URL - both are read from there,
// never typed in by the user.
// ============================================================

document.addEventListener("DOMContentLoaded", () => {
  const params = new URLSearchParams(window.location.search);
  const userEmail = params.get("email");
  const resetToken = params.get("token");

  const form = document.getElementById("resetPasswordForm");

  if (!userEmail || !resetToken) {
    form.innerHTML = '<p>This reset link is missing information. Please request a new one from the <a href="forgot-password.html">forgot password page</a>.</p>';
    return;
  }

  form.addEventListener("submit", async (event) => {
    event.preventDefault();

    const newPassword = document.getElementById("newPassword").value;

    try {
      await resetPassword(userEmail, resetToken, newPassword);
      // Redirect to login instead of alert()-ing here - login.html/auth.js
      // shows a banner when it sees ?passwordReset=1, same pattern as the
      // ?justReserved=1 banner on my-reservations.html.
      window.location.href = "login.html?passwordReset=1";
    } catch (err) {
      alert("Could not reset password: " + err.message);
    }
  });
});
