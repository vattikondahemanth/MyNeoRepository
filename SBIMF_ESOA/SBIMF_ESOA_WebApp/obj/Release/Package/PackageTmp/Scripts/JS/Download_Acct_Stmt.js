            var loader = {};
            loader = {
                start: function () {
                    $('#SiteLoader').addClass('overlay-loader').append($('<div>').addClass('loader'));
                    },
                stop: function () {
                    $('#SiteLoader').removeClass('overlay-loader').empty();
                }
            }

$(document).ready(function () {

                forgeryId = $("#forgeryToken").val();

                //$('input[type=datetime]').datepicker({
                //    dateFormat: "dd/M/yy",
                //    changeMonth: true,
                //    changeYear: true,
                //    yearRange: "-60:+0",
                //    maxDate: new Date()
                //});

                //setting from date to current fiscal year start date
                var today = new Date();
                var curMonth = today.getMonth();
                var loadFirst = 1;

                var fiscalDt = "";
                if (curMonth > 3) {
                    fiscalDt = '01/Apr/' + today.getFullYear().toString();
                }
                else {
                    fiscalDt = '01/Apr/' + (today.getFullYear() - 1).toString();
                }

                var month_names = ["Jan", "Feb", "Mar",
                      "Apr", "May", "Jun",
                      "Jul", "Aug", "Sep",
                      "Oct", "Nov", "Dec"];

                var curr_dt = today.getDate() + '/' + month_names[today.getMonth()] + '/' + today.getFullYear();
                //end: setting from date to current fiscal year start date

                //$("#dtFrom").datepicker("setDate", fiscalDt);
                //$("#dtTo").datepicker("setDate", today);

                //Date range picker
                var start = fiscalDt;
                var end = moment();

                var startDt, endDt;

                $('#reportrange').daterangepicker({
                    locale: {
                        format: 'DD/MMM/YYYY'
                    },
                    startDate: start,
                    endDate: end,
                    maxDate: new Date(),
                    ranges: {
                        'Last 3 Months': [moment().subtract(3, 'month')],
                        'Last 6 Months': [moment().subtract(6, 'month')],
                        'Last 1 year': [moment().subtract(1, 'year')]
                    }
                }, cb);
                cb(start, end);

                function cb(start, end) {
                    if (loadFirst == 1) {
                        loadFirst = 0;
                        startDt = fiscalDt;
                        endDt = curr_dt;
                        $('#reportrange span').html(startDt + ' - ' + endDt);
                    }
                    else {
                        startDt = start.format('DD/MMM/YYYY');
                        endDt = end.format('DD/MMM/YYYY');
                        $('#reportrange span').html(start.format('DD/MMM/YYYY') + ' - ' + end.format('DD/MMM/YYYY'));
                    }
                }


                loader.stop();

                $("#btnGenStmtPDF").click(function () {
                    genStmtPdf(startDt, endDt);
                });

            });

            function genStmtPdf(dateFrom, dateTo) {

                var body = JSON.stringify({ dateFrom: dateFrom, dateTo: dateTo });
                var ran_key = CryptoJS.enc.Utf8.parse(key);
                var iv = CryptoJS.enc.Utf8.parse(key);
                var encryptedbody = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(body), ran_key,
                    {
                        keySize: 128 / 8,
                        iv: iv,
                        mode: CryptoJS.mode.CBC,
                        padding: CryptoJS.pad.Pkcs7
                    }).toString();

                loader.start();

                $.ajax({
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json',
                    url: GenerateAcctStmtPdfUrl,
                    headers: {
                        'VerificationToken': forgeryId
                    },
                    data: JSON.stringify({ body: encryptedbody }),
                    success: function (result) {
                        debugger;
                        var decrypteddata = CryptoJS.AES.decrypt(result.ResponseBody, ran_key, {
                            iv: iv,
                            padding: CryptoJS.pad.Pkcs7,
                            mode: CryptoJS.mode.CBC
                        });
                        decrypteddata = decrypteddata.toString(CryptoJS.enc.Utf8)
                        decrypteddata = JSON.parse(decrypteddata);
                        var res = decrypteddata.res;
                        if (res == "Success") {
                            loader.stop();
                            $("#lblErrMsg").css("display", "none");
                            window.location.href = DownloadStmtUrl;
                        }
                        else if (res == "Fail") {
                            loader.stop();
                            $("#lblErrMsg").css("display", "block");
                            $("#lblErrMsg").text(decrypteddata.ErrMessage);
                        }
                        else if (res == "Error") {
                            loader.stop();
                            alert(decrypteddata.ErrMessage);
                        }

                        loader.stop();
                    },
                    error: function (xhr, status, error) {
                        loader.stop();
                        alert("Error loading data for fund category chart! Please try again.");
                    }

                });

            }


