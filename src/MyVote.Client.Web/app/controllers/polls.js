/// <reference path="../_refs.ts" />
var MyVote;
(function (MyVote) {
    (function (Controllers) {
        'use strict';

        var PollsCtrl = (function () {
            function PollsCtrl($scope, $location, myVoteService) {
                $scope.filters = MyVote.Models.PollFilters;
                $scope.filterBy = $scope.filters[0];

                $scope.$watch('filterBy', function () {
                    $scope.pollGroups = null;
                    $scope.message = 'Loading polls...';
                    myVoteService.getPolls($scope.filterBy.value).then(function (result) {
                        $scope.message = result ? null : "There are no polls!";
                        $scope.pollGroups = result;
                    });
                });

                $scope.responseText = function (count) {
                    return count == 1 ? "Response" : "Responses";
                };

                $scope.addPoll = function () {
                    $location.path('/addPoll');
                };

                $scope.viewPoll = function (id) {
                    $location.path('/viewPoll/' + id);
                };

                $scope.getBackImg = function (imageLink) {
                    return imageLink && imageLink != '0' ? imageLink : '/Content/checkmark.svg';
                };
            }
            PollsCtrl.$inject = ['$scope', '$location', 'myVoteService'];
            return PollsCtrl;
        })();
        Controllers.PollsCtrl = PollsCtrl;
    })(MyVote.Controllers || (MyVote.Controllers = {}));
    var Controllers = MyVote.Controllers;
})(MyVote || (MyVote = {}));
