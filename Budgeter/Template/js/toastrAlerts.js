
//Top Right Notifications
function SuccessNotification(title, body) {
    var msgTitle = title;
    var msgBody = body;
    $.toast({
        heading: msgTitle,
        text: msgBody,
        position: 'top-right',
        loaderBg:'#ff6849',
        icon: 'success',
        hideAfter: 6000, 
        stack: 6
    });
};

function InfoNotification() {
    $.toast({
        heading: 'Welcome to my Elite admin',
        text: 'Use the predefined ones, or specify a custom position object.',
        position: 'top-right',
        loaderBg: '#ff6849',
        icon: 'info',
        hideAfter: 7000,
        stack: 6
    });
};

function ErrorNotification(title, body) {
    var ErrorTitle = title;
    var ErrorBody = body;
    $.toast({
        heading: ErrorTitle,
        text: ErrorBody,
        position: 'top-right',
        loaderBg: '#ff6849',
        icon: 'error',
        hideAfter: 7000,
        stack: 6

    });
};

function AccOverdraftNotification(title, body) {
    var ErrorTitle = title;
    var ErrorBody = body;
    $.toast({
        heading: ErrorTitle,
        text: ErrorBody,
        position: 'top-right',
        loaderBg: '#ff6849',
        icon: 'warning',
        hideAfter: 16000,
        stack: 6
    });

};