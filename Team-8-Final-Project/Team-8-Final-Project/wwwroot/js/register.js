// ============================================================
// register.js — handles the sign-up form on register.html
// ============================================================

document.addEventListener("DOMContentLoaded", () => {
  const form = document.getElementById("registerForm");

  form.addEventListener("submit", async (event) => {
    event.preventDefault(); // stop the browser's default full-page form submit

    const user = {
      firstName: document.getElementById("firstName").value,
      lastName: document.getElementById("lastName").value,
      userEmail: document.getElementById("userEmail").value,
      userPhoneNo: document.getElementById("userPhoneNo").value,
      dob: document.getElementById("dob").value,
      password: document.getElementById("password").value
    };

    try {
      await register(user);
      alert("Account created! You can log in now.");
      window.location.href = "login.html";
    } catch (err) {
      alert("Could not register: " + err.message);
    }
  });
});
