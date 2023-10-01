$(function () {

    $(document).ready(function () {
        $(document).on("click", ".deleteImage", function () {
            $(this).parent().remove();
        })
    })

})

let posterInput = document.getElementById("posterInput")
let posterBox = document.getElementById("poster-image-box")



const imagePosterInput = document.getElementById('imagePosterInput');
const posterImageBox = document.getElementById('posterimage-box');

imagePosterInput.addEventListener('change', function (e) {
    const file = e.target.files[0];
    const reader = new FileReader();

    reader.onload = function (e) {
        const imageBox = posterImageBox.querySelector('.image-box');
        const image = imageBox.querySelector('img');

        image.src = e.target.result;
    };

    if (file) {
        reader.readAsDataURL(file);
    }
});


