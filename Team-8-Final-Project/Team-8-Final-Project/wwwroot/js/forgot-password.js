// ============================================================
// forgot-password.js — handles the form on forgot-password.html
// ============================================================

document.addEventListener("DOMContentLoaded", () => {
  const form = document.getElementById("forgotPasswordForm");

  form.addEventListener("submit", async (event) => {
    event.preventDefault();

    const userEmail = document.getElementById("userEmail").value;

    try {
      await forgotPassword(userEmail);
      // Always goes to the same confirmation page whether or not the email
      // actually matched an account - that's intentional on the backend
      // (AuthController doesn't say "no account found" here), so it doesn't
      // let someone probe which emails are registered.
      window.location.href = "check-email.html";
    } catch (err) {
      alert("Something went wrong: " + err.message);
    }
  });
});
