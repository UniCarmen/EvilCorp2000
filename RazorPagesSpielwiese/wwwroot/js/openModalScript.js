
document.addEventListener("DOMContentLoaded", function () {
    const modalStateElement = document.getElementById("modalState");
    const modalState = modalStateElement.getAttribute("data-modal-state");

    if (modalState === "New") {
        const modalElement = document.getElementById("newProductModal");
        if (modalElement) {
            const modal = new bootstrap.Modal(modalElement);
            modal.show();
        }
    }

    if (modalState === "Alter") {
        const modalElement = document.getElementById("alterProductModal");
        if (modalElement) {
            const modal = new bootstrap.Modal(modalElement);
            modal.show();
            document.body.classList.add('modal-open');
        }
    }

});


