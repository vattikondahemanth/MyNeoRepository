if (jQuery)
    (function ($) {
        $.widget('idealake.exponentialslider', $.ui.slider, {
            options: { slidertype: 'linear', logicalmin: 0, logicalmax: 10000, logicalstep: 100, finalvalue: 0 },
            _log10: function (num) {
                return Math.round(Math.log(num) / Math.LN10)
            },
            _calculatemax: function () {
                var min = this.options.logicalmin == 0 ? this.options.logicalstep : this.options.logicalmin;
                //  var max = (Math.round((this._log10(this.options.logicalmax) - this._log10(min))) * 9) + 1;
                var max = this.maxvalue(this.options.logicalmax);
                return max;
            },
            _create: function () {
                var self = this;
                this.options.range = "min";
                if (this.options.slidertype !== 'linear') {
                    this.options.min = this.options.logicalmin == 0 ? 0 : 1;
                    this.options.max = this._calculatemax();
                    this.options.step = 1;
                }
                else {
                    this.options.min = this.options.logicalmin;
                    this.options.max = this.options.logicalmax;
                    this.options.step = this.options.logicalstep;
                }

                this.options.slide = function (event, ui) {
                    if (self.options.slidertype !== 'linear') {
                        var pvalue = ui.value;
                        var x = Math.floor((pvalue - 1) / 9);
                        var w = parseInt((pvalue - 1) % 9) + 1;
                        var log10step = Math.round(Math.log(self.options.logicalstep) / Math.LN10);
                        var finalValue = pvalue == 0 ? 0 : Math.pow(10, (x + log10step)) * w;
                        ui.finalvalue = finalValue;
                    }
                    else {
                        ui.finalvalue = ui.value;
                    }

                    self._trigger("exponentialslide", event, ui);
                };

                this._super();
            },
            finalvalue: function (value) {
                if (value) {
                    if (this.options.slidertype != 'linear') {
                        //var digits = String(value).length;
                        //var closestTen = Math.pow(10, digits - 1);
                        //var factor = parseInt(value / closestTen);
                        //value = factor * closestTen;
                        //this.options.finalvalue = value;
                        //var valueLog = Math.log(value) / Math.LN10;
                        //var sliderminvalue = this.options.logicalmin == 0 ? this.options.logicalstep : this.options.logicalmin;
                        //var slideminvaluelog = Math.log(sliderminvalue) / Math.LN10;

                        //value = 9 * (Math.floor((9 * (valueLog - slideminvaluelog) + 1 - 1) / 9)) + (value / Math.pow(10, (Math.round(slideminvaluelog) + (Math.floor((9 * (valueLog - slideminvaluelog) + 1 - 1) / 9)))));
                        value = this.maxvalue(value);
                    }
                    this.value(value);

                }
                else {
                    if (this.options.slidertype != 'linear') {
                        return this.options.finalvalue;
                    }
                    else {
                        return this.options.value;
                    }
                }
            },
            maxvalue: function (value) {
                var digits = String(value).length;
                var closestTen = Math.pow(10, digits - 1);
                var factor = parseInt(value / closestTen);
                value = factor * closestTen;
                if (value != 0) {

                    this.options.finalvalue = value;
                    var valueLog = Math.log(value) / Math.LN10;
                    var sliderminvalue = this.options.logicalmin == 0 ? this.options.logicalstep : this.options.logicalmin;
                    var slideminvaluelog = Math.log(sliderminvalue) / Math.LN10;
                    value = 9 * (Math.floor((9 * (valueLog - slideminvaluelog) + 1 - 1) / 9)) + (value / Math.pow(10, (Math.round(slideminvaluelog) + (Math.floor((9 * (valueLog - slideminvaluelog) + 1 - 1) / 9)))));
                }
                return value;
            }




        });
    })(jQuery);