const ImagePreviewer = {
    settings: {
        preview: document.querySelector('#preview-image'),
        fileReader: new FileReader(),
        fileControl: document.querySelector('.form-control-file')
    },
    initialize: function () {
        this.bindUIEvents();
    },
    bindUIEvents: function () {
        ImagePreviewer.settings.fileControl.onchange = ImagePreviewer.preview;
    },
    preview: function () {
        var file = ImagePreviewer.settings.fileControl.files[0];
    
        ImagePreviewer.settings.fileReader.onloadend = function () {
            ImagePreviewer.settings.preview.src = ImagePreviewer.settings.fileReader.result;
        }
    
        if (file) {
            ImagePreviewer.settings.fileReader.readAsDataURL(file);
        } else {
            ImagePreviewer.settings.preview.src = "";
        }
    }
}

$( ImagePreviewer.initialize() );