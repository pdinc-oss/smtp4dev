define(["knockout", "api", "toastr", "text!./viewmessageview.html"],
    function (ko, api, toastr, template) {
        function ViewMessageViewModel(params) {
            var self = this;
            self.body = ko.observable();

            self.loadBody = function () {
                self.loading(true);
                api.Message.find(self.id).then(
                    function (data) {
                        self.body(data.body);
                        self.loading(false);
                    },
                function () {
                    self.loading(false);
                });
            };

            self.loading = ko.observable(false);
            self.id = ko.observable();

            self.refresh = function () {
                self.loading(true);

                self.id = params.id;
                self.loadBody();
                self.loading(false);
            };

            self.refresh();
        }

        return {
            viewModel: { viewModel: ViewMessageViewModel },
            template: template
        };
    }
);