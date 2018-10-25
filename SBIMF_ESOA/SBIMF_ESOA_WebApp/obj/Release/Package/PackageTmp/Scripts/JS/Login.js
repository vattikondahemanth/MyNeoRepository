var LoginUrl
    , Encrypt_PwdUrl
    , HomeUrl
    , FundChartUrl
    , GetSchemeChartUrl
    , SchemeChartUrl
    , SchemeChartDataUrl
    , GetSchemeD_ChartUrl
    , DisplaySchemeD_ChartUrl
    , DisplaySchemeD_Chart_DataUrl
    , GetAcctStmtDetailsUrl
    , GenerateAcctStmtPdfUrl        

var loader = {};
loader = {
    start: function () {
        $('#SiteLoader').addClass('overlay-loader').append($('<div>').addClass('loader'));
    },
    stop: function () {
        $('#SiteLoader').removeClass('overlay-loader').empty();
    }
}





window.onload = function () {
    loader.stop();  
    var myInput = document.getElementById('txtPwd');
    myInput.onpaste = function (e) {
        loader.stop();  
        e.preventDefault();
    }
}


$(document).ready(function () {

    //loader.stop();  

    $('#txtPwd').keyup(function (event) {
        var txtPassword = $(this).val();
        re = /[`~!@#$%^&*()_|+\-=?;:'",.<>\{\}\[\]\\\/]/gi;
        var isSplChar = re.test(txtPassword);
        if (isSplChar) {
            var no_spl_char = txtPassword.replace(/[`~!@#$%^&*()_|+\-=?;:'",.<>\{\}\[\]\\\/]/gi, '');
            $(this).val(no_spl_char);
        }
    });
    


    $("#btnLogin").click(function () {
        var commonObj = new CommonJS();
        debugger;
        forgeryId = $("#forgeryToken").val();
        var hashes = window.location.href.indexOf('?');

        if (hashes == -1) {
            $("#txtPwd").val('');
            $("#spError").text("Please enter valid URL.");
            $("#spError").css("display", "block");
        }
        else {


            loader.start();
            if ($("#txtPwd").val() == "" || $("#txtPwd").val() == null) {
                $("#spError").text("Please Enter Password.");
                $("#spError").css("display", "block");
                loader.stop();
            }
            else {

                var txtpassword = $('#txtPwd').val();
                var ran_key = CryptoJS.enc.Utf8.parse(key);
                var iv = CryptoJS.enc.Utf8.parse(key);
                var encryptedpwd = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(txtpassword), ran_key,
                    {
                        keySize: 128 / 8,
                        iv: iv,
                        mode: CryptoJS.mode.CBC,
                        padding: CryptoJS.pad.Pkcs7
                    }).toString();

                var body = JSON.stringify({ HDPass: encryptedpwd });
                $.when(commonObj.apiHelper(LoginUrl, 'POST', body)).then(function (data) {
                    var msg = data;
                    if (msg == "Success") {
                        loader.stop();
                        $("#spError").text('');
                        $("#spError").css("display", "none");
                        window.location.href = HomeUrl;
                    }
                    else if (msg.includes("remaining.")) {
                        loader.stop();
                        $("#txtPwd").val('');
                        $("#spError").text('Incorrect Password. ' + msg);
                        $("#spError").css("display", "block");
                    }
                    else if (msg.includes("All attempts are over.")) {
                        loader.stop();
                        $("#txtPwd").val('');
                        $("#txtPwd").attr("disabled", "disabled");
                        $("#spError").text(msg);
                        $("#spError").css("display", "block");
                    }
                    else if (msg == "NoFolio") {
                        loader.stop();
                        $("#txtPwd").val('');
                        $("#spError").text("Folio does not exists.");
                        $("#spError").css("display", "block");
                    }
                    else {
                        loader.stop();
                        $("#txtPwd").val('');
                        $("#spError").text("Unsuccessful Login!");
                        $("#spError").css("display", "block");
                    }
                }, function (errorThrown) {
                    alert(errorThrown);
                });


                //$.ajax({
                //    type: 'POST',
                //    dataType: 'json',
                //    contentType: 'application/json',
                //    url: LoginUrl,
                //    data: JSON.stringify({ body: encryptedbody }),
                //    headers: {
                //        'VerificationToken': forgeryId
                //    },
                //    success: function (data) {
                //        var decrypteddata = CryptoJS.AES.decrypt(data.ResponseBody, ran_key, {
                //            iv: iv,
                //            padding: CryptoJS.pad.Pkcs7,
                //            mode: CryptoJS.mode.CBC
                //        });
                //        decrypteddata = decrypteddata.toString(CryptoJS.enc.Utf8)

                //        var msg = decrypteddata;
                //        if (msg == "Success") {
                //            loader.stop();
                //            $("#spError").text('');
                //            $("#spError").css("display", "none");
                //            window.location.href = HomeUrl;
                //        }
                //        else if (msg.includes("remaining.")) {
                //            loader.stop();
                //            $("#txtPwd").val('');
                //            $("#spError").text('Incorrect Password. ' + msg);
                //            $("#spError").css("display", "block");
                //        }
                //        else if (msg.includes("All attempts are over.")) {
                //            loader.stop();
                //            $("#txtPwd").val('');
                //            $("#txtPwd").attr("disabled", "disabled");
                //            $("#spError").text(msg);
                //            $("#spError").css("display", "block");
                //        }
                //        else if (msg == "NoFolio") {
                //            loader.stop();
                //            $("#txtPwd").val('');
                //            $("#spError").text("Folio does not exists.");
                //            $("#spError").css("display", "block");
                //        }
                //        else {
                //            loader.stop();
                //            $("#txtPwd").val('');
                //            $("#spError").text("Unsuccessful Login!");
                //            $("#spError").css("display", "block");
                //        }
                //    },
                //    error: function () {
                //    }
                //});




            }
        }
    });
});