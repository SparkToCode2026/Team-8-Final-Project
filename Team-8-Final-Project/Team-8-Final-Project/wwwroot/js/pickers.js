// ============================================================
// pickers.js — shared "search by name/title, click to pick" widget
//
// Used anywhere a staff form needs to reference another record (a book, a
// book copy, a member) so staff can search instead of memorizing or looking
// up a numeric ID. Load this after api.js and before the page's own script
// on any page that uses createSearchPicker().
//
// createSearchPicker(options) turns an empty container element into a text
// search box + hidden "selected id" input + a dropdown of matches that stays
// closed until the box is focused or typed into. Returns
// { getSelectedId(), getSelectedItem(), reset() } for the page's own script
// to call when reading the form or clearing it after a successful submit.
//
// options:
//   containerId - id of an empty <div> already in the page's HTML
//   items       - function returning the current array to search (a
//                 function, not a plain array, so it always reads whatever
//                 is currently loaded even if that list refreshes later)
//   getId       - (item) => the value to use as this item's id
//   getLabel    - (item) => the text shown for this item, both in the
//                 dropdown and filled into the search box once picked
//   placeholder - placeholder text for the search box
//   onSelect    - optional extra callback run with the picked item
// ============================================================

function createSearchPicker({ containerId, items, getId, getLabel, placeholder, onSelect }) {
  const container = document.getElementById(containerId);
  if (!container) return { getSelectedId: () => null, getSelectedItem: () => null, reset: () => {} };

  container.classList.add("position-relative");
  container.innerHTML = `
    <input type="text" class="form-control picker-search" placeholder="${placeholder}" autocomplete="off">
    <input type="hidden" class="picker-value">
    <div class="picker-results list-group position-absolute w-100" style="z-index: 1000; max-height: 220px; overflow-y: auto; display: none;"></div>
  `;

  const searchInput = container.querySelector(".picker-search");
  const hiddenInput = container.querySelector(".picker-value");
  const resultsBox = container.querySelector(".picker-results");
  let selectedItem = null;
  let currentMatches = [];

  function renderResults() {
    const query = searchInput.value.trim().toLowerCase();
    const list = items() || [];

    currentMatches = query
      ? list.filter(item => getLabel(item).toLowerCase().includes(query)).slice(0, 8)
      : list.slice(0, 8);

    resultsBox.innerHTML = currentMatches.length
      ? currentMatches.map((item, index) => `<button type="button" class="list-group-item list-group-item-action picker-result" data-index="${index}">${getLabel(item)}</button>`).join("")
      : '<div class="list-group-item text-muted">No matches</div>';

    resultsBox.querySelectorAll(".picker-result").forEach(btn => {
      btn.addEventListener("click", () => {
        const item = currentMatches[Number(btn.dataset.index)];
        selectedItem = item;
        searchInput.value = getLabel(item);
        hiddenInput.value = getId(item);
        resultsBox.style.display = "none";
        if (onSelect) onSelect(item);
      });
    });

    resultsBox.style.display = "block";
  }

  searchInput.addEventListener("focus", renderResults);
  searchInput.addEventListener("input", () => {
    hiddenInput.value = "";
    selectedItem = null;
    renderResults();
  });

  document.addEventListener("click", (event) => {
    if (!container.contains(event.target)) {
      resultsBox.style.display = "none";
    }
  });

  return {
    getSelectedId: () => hiddenInput.value || null,
    getSelectedItem: () => selectedItem,
    reset: () => {
      searchInput.value = "";
      hiddenInput.value = "";
      selectedItem = null;
      resultsBox.innerHTML = "";
      resultsBox.style.display = "none";
    }
  };
}

// Small helper for pulling an id off an object whose exact field name/casing
// isn't guaranteed - this backend has been inconsistent about Id vs ID
// casing across different models (UserId vs UserID, etc.), so anything that
// needs a record's id checks every reasonable casing before giving up.
function extractId(obj, ...keys) {
  if (!obj) return null;
  for (const key of keys) {
    if (obj[key] !== undefined && obj[key] !== null) return obj[key];
  }
  return null;
}

// NOTE: deliberately NOT named getUserId() - api.js already defines a
// getUserId() that decodes the current logged-in user's id from the JWT,
// and both scripts load globally on the same page, so reusing that name
// here would silently overwrite it and break login/role detection sitewide.
function extractUserRecordId(userLike) {
  return extractId(userLike, "userId", "userID", "UserId", "UserID", "id", "Id", "ID");
}
