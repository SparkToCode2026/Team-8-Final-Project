// ============================================================
// auth.js — handles the login form on login.html
// (register.html would need a similar handler for POST /Auth/Register)
// ============================================================
 
document.addEventListener("DOMContentLoaded", () => {
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
 
      // Send each role to its own dashboard
      const role = getUserRole();
      if (role === "Admin") {
        window.location.href = "admin/dashboard.html";
      } else if (role === "Librarian") {
        window.location.href = "staff/dashboard.html";
      } else {
        window.location.href = "member/dashboard.html";
      }
    } catch (err) {
      alert(err.message);
    }
  });
});