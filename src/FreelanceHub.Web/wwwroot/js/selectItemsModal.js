/**
 * selectItemsModal.js
 * 
 * Shared JavaScript utilities for the _SelectItemsModal component.
 * Handles opening modals, searching/filtering items, and collecting selected IDs.
 */

// Store selected items by modal ID
const selectedItems = {};

/**
 * Opens a modal for selecting items and initializes search functionality.
 * 
 * @param {string} modalId - The ID of the modal to open (e.g., "categoryModal")
 * @param {string} itemType - The type of items ("category", "tag", "skill")
 * @param {function} callback - Optional callback function to invoke after confirming selection
 */
function openSelectModal(modalId, itemType, callback) {
    // Initialize storage for this modal if not already done
    if (!selectedItems[modalId]) {
        selectedItems[modalId] = {
            type: itemType,
            ids: [],
            names: [],
            callback: callback
        };
    } else {
        selectedItems[modalId].callback = callback;
    }

    // Setup search filter
    setupSearchFilter(modalId, itemType);

    // Show the modal (Bootstrap 5)
    const modal = new bootstrap.Modal(document.getElementById(modalId));
    modal.show();
}

/**
 * Sets up the search/filter functionality for the modal.
 * Filters items in real-time as the user types.
 * 
 * @param {string} modalId - The ID of the modal
 * @param {string} itemType - The type of items
 */
function setupSearchFilter(modalId, itemType) {
    const searchInput = document.getElementById(modalId + "_search");
    const listContainer = document.getElementById(modalId + "_list");
    const items = listContainer.querySelectorAll(".list-group-item");

    if (!searchInput) return;

    searchInput.oninput = function () {
        const query = this.value.toLowerCase();
        items.forEach(item => {
            const label = item.textContent.toLowerCase();
            item.style.display = label.includes(query) ? "" : "none";
        });
    };

    // Clear search when modal is hidden
    const modal = document.getElementById(modalId);
    modal.addEventListener("hidden.bs.modal", function () {
        searchInput.value = "";
        items.forEach(item => item.style.display = "");
    });
}

/**
 * Collects all selected item IDs and names from the modal.
 * 
 * @param {string} modalId - The ID of the modal
 * @param {string} itemType - The type of items ("category", "tag", "skill")
 * @returns {object} Object with 'ids' array and 'names' array
 */
function getSelectedIds(modalId, itemType) {
    const checkboxes = document.querySelectorAll(`.${itemType}Item[data-modal="${modalId}"]:checked`);
    const ids = [];
    const names = [];

    checkboxes.forEach(checkbox => {
        ids.push(parseInt(checkbox.value));
        names.push(checkbox.getAttribute("data-name"));
    });

    return { ids, names };
}

/**
 * Confirms the selection and closes the modal.
 * Calls the associated callback (if provided) with selected IDs and names.
 * 
 * @param {string} modalId - The ID of the modal
 * @param {string} itemType - The type of items ("category", "tag", "skill")
 */
function confirmSelection(modalId, itemType) {
    const selected = getSelectedIds(modalId, itemType);
    
    // Store the selection
    if (selectedItems[modalId]) {
        selectedItems[modalId].ids = selected.ids;
        selectedItems[modalId].names = selected.names;
    }

    // Call the callback if provided
    if (selectedItems[modalId] && typeof selectedItems[modalId].callback === "function") {
        selectedItems[modalId].callback(selected.ids, selected.names);
    }

    // Close the modal
    const modal = bootstrap.Modal.getInstance(document.getElementById(modalId));
    if (modal) {
        modal.hide();
    }
}

/**
 * Gets all currently selected IDs and names for a modal.
 * Useful if you need to check selections without closing the modal.
 * 
 * @param {string} modalId - The ID of the modal
 * @returns {object} Object with 'ids' array and 'names' array
 */
function getStoredSelection(modalId) {
    return selectedItems[modalId] ? {
        ids: selectedItems[modalId].ids,
        names: selectedItems[modalId].names
    } : { ids: [], names: [] };
}

/**
 * Clears all selections in a modal.
 * 
 * @param {string} modalId - The ID of the modal
 * @param {string} itemType - The type of items ("category", "tag", "skill")
 */
function clearSelection(modalId, itemType) {
    const checkboxes = document.querySelectorAll(`.${itemType}Item[data-modal="${modalId}"]`);
    checkboxes.forEach(checkbox => {
        checkbox.checked = false;
    });

    if (selectedItems[modalId]) {
        selectedItems[modalId].ids = [];
        selectedItems[modalId].names = [];
    }
}

/**
 * Pre-selects items in a modal by their IDs.
 * Useful for editing existing data.
 * 
 * @param {string} modalId - The ID of the modal
 * @param {string} itemType - The type of items ("category", "tag", "skill")
 * @param {array} idsToSelect - Array of IDs to pre-check
 */
function preSelectItems(modalId, itemType, idsToSelect) {
    const checkboxes = document.querySelectorAll(`.${itemType}Item[data-modal="${modalId}"]`);
    checkboxes.forEach(checkbox => {
        const id = parseInt(checkbox.value);
        checkbox.checked = idsToSelect.includes(id);
    });
}
