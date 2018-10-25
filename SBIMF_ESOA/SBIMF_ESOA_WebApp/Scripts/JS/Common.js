var CommonJS = function () {
    var ran_key = CryptoJS.enc.Utf8.parse(key),

        iv = CryptoJS.enc.Utf8.parse(key),

        encryptData = function (value) {
            return CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(value), ran_key,
                {
                    keySize: 128 / 8,
                    iv: iv,
                    mode: CryptoJS.mode.CBC,
                    padding: CryptoJS.pad.Pkcs7
                }).toString();
        },

        decryptData = function (value) {
            var decrypteddata = CryptoJS.AES.decrypt(value.ResponseBody, ran_key, {
                iv: iv,
                padding: CryptoJS.pad.Pkcs7,
                mode: CryptoJS.mode.CBC
            });
            decrypteddata = decrypteddata.toString(CryptoJS.enc.Utf8)

            return decrypteddata;
        },

        apiHelper = function (uri, method, requestData) {
            //self.error(''); // Clear error message
            debugger;
            var apideffer = $.Deferred();
            $.ajax({
                type: method,
                dataType: 'json',
                contentType: 'application/json',
                url: uri,
                data: JSON.stringify({ body: encryptData(requestData) }),//JSON.stringify({ body: encryptedbody }),
                headers: {
                    'VerificationToken': forgeryId
                },
                success: function (result) {
                    apideffer.resolve(decryptData(result));
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                apideffer.reject(errorThrown);
                });
           return apideffer.promise();
        };

    return {
        apiHelper: apiHelper
    }
};

