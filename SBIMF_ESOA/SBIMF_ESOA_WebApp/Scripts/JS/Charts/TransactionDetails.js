var loader = {};
loader = {
    start: function () {
        $('#SiteLoader').addClass('overlay-loader').append($('<div>').addClass('loader'));
    },
    stop: function () {
        $('#SiteLoader').removeClass('overlay-loader').empty();
    }
}

$.fn.scrollView = function () {
    return this.each(function () {
        $('html, body').animate({
            scrollTop: $(this).offset().top
        }, 1000);
    });
}

$(document).ready(function () {

    forgeryId = $("#forgeryToken").val();

    $.ajax({
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        url: DisplaySchemeD_Chart_DataUrl,
        headers: {
            'VerificationToken': forgeryId
        },
        data: {},
        success: function (data) {
            var ran_key = CryptoJS.enc.Utf8.parse(key);
            var iv = CryptoJS.enc.Utf8.parse(key);
            var decrypteddata = CryptoJS.AES.decrypt(data.ResponseBody, ran_key, {
                iv: iv,
                padding: CryptoJS.pad.Pkcs7,
                mode: CryptoJS.mode.CBC
            });
            decrypteddata = decrypteddata.toString(CryptoJS.enc.Utf8)
            decrypteddata = JSON.parse(decrypteddata);
            $('#dvTransactions').scrollView();
            var lst_Schemes = decrypteddata.lstSchemes;
                    
            $("#sp_schName").text(lst_Schemes.Name);
            $("#sp_schCostValue").text(lst_Schemes.costvalue);
            $("#sp_schCurrValue").text(lst_Schemes.currentvalue);
            $("#sp_schGL").text(lst_Schemes.gain_loss);
            $("#sp_schGLVal").text(lst_Schemes.gain_loss_value);
                    
            if (lst_Schemes.gain_loss == "Gain") {
                $(".GL_Col").css("color", "#4BB543");
            }
            else if (lst_Schemes.gain_loss == "Loss") {
                $(".GL_Col").css("color", "#ed5249");
            }
            else {
                $(".GL_Col").css("color", "#0095da");
            }

            var lst_Trans = decrypteddata.lstTrns;
            $('#tblTrans').DataTable({
                responsive: true,
                data: lst_Trans,
                "ordering": true,
                columns: [
                    { "data": "Date", title: "Date" },
                    { "data": "Type", title: "Transaction Type" },
                    { "data": "Amount", title: "Amount" },
                    { "data": "Nav", title: "NAV in INR (Rs.)" },
                    { "data": "Price", title: "Price in INR (Rs.)" },
                    { "data": "N_units", title: "Number of Units" },
                    { "data": "B_units", title: "Balance Unit" }
                ]
            });
            loader.stop();
        },
        error: function (xhr, status, error) {
            loader.stop();
            alert("Error loading data for scheme transactions!")
        }
    });
});
