
document.addEventListener("DOMContentLoaded", function () {
    const modalStateElement = document.getElementById("modalState");
    const modalState = modalStateElement.getAttribute("data-modal-state");



    if (modalState === "True") {
        const modalElement = document.getElementById("newAndAlterProductModal");
        if (modalElement) {
            const modal = new bootstrap.Modal(modalElement);
            modal.show();
        }
    }

});


