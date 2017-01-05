angular.module('weChatDemoApp', ['ngAnimate', 'ui.bootstrap']);

angular.module('weChatDemoApp').controller('homeController', ['$scope', function ($scope) {
    $scope.slideIndex = 0;
}]);

angular.module('weChatDemoApp').controller('aboutController', ['$scope', function ($scope) {
}]);

angular.module('weChatDemoApp').controller('itemController', ['$scope', 'itemId', function ($scope, itemId) {
    $scope.rate = 2;

    $scope.titles = ["Great Job in the Brazil TM implementation project"
                    , "Great Effort Supporting Machinery Commissioning"
                                                            , "Quickly quench a flame at Qingpu PHG"
                                                            , "工廠跳車,隨call隨到"
                                                            , "15 Years of Service"];

    $scope.nominateds = ["Vijaya Keerthi Penmetsa"
                        , "Kevin Mc Colgan"
                        , "Peng Jet"
                        , "Adonis Li-Ta Huang"
                        , "Annie Ramsey"];

    $scope.nominates = ["Cherish Zhang"
                        , "Ghassan Altimany"
                        , "Hulin Wang"
                        , "Andersen Chaur Yih Lu"
                        , "Michael Sicinski"];

    $scope.descriptions = ["Keerthi did an excellent job in the Brazil TM implementation project. Keerthi is very quick in fixing defects and addressing issues. There is a spec change after I see the live credit card feed from the bank, some unexpected transactions are sent in the statement, which we do not want to import into SAP. Keerthi made changes to the spec changes very quickly. It's impossible for the project team to deliver the project successfully without Keerthi's help! Great Job!"
                            , "Thank you for leading, communicating all machinery commissioning issues at BRS, and working hard with my team to resolve when you are extremely busy and involved in the whole plant on-time start-up. True leader, great Job and Thank You !! "
                            , "On 29th July Peng Jet worked a night shift at Shanghai Qingpu PHG site, he found a flame at vent point(H2 was ignited by static eletricity), he report to manager and quench a fire by water quickly, which eliminated a big safety risk and no any influence to both AP and customer. As an operator leader, in past years he always work diligently and actively, and already found and resolved several big issue and risk. Appreciate his significant contribution "
                                , "感謝中區PMO振德,嘉雄以及立達三位同仁,於2016/8/17中午接獲SPIL2P2因台電壓降造成跳車,必須儘速利用空檔更換第二段出口Gasket時,安全迅速的抵達廠內,且在雨中協助搶修.讓公司可以降低液氮的損耗.也使得我們的團隊,受到客戶更加肯定.非常感謝."
                                , "Annie, It has been enjoyable working with you since returning from Asia. You have been an excellent leader and mentor to your Process Systems team. I will always ... "];

    $scope.picurls = ["/images/us1.jpg"
                        , "/images/us2.jpg"
                        , "/images/us3.jpg"
                        , "/images/us4.jpg"
                        , "/images/us5.jpg"];

    $scope.stags = ["Self-Confidence"
                        ,"Inclusion"
                        , "Safty"
                        , "Speed"
                        , "Self-Confidence"];

    $scope.dates = ["2016-8-18"
                    ,"2016-8-22"
                        , "2016-8-18"
                        , "2016-8-21"
                        , "2016-8-17"];

    $scope.title = $scope.titles[itemId];
    $scope.description = $scope.descriptions[itemId];
    $scope.picurl = $scope.picurls[itemId];
    $scope.nominate = $scope.nominates[itemId];
    $scope.nominated = $scope.nominateds[itemId];
    $scope.stag = $scope.stags[itemId];
    $scope.date = $scope.dates[itemId];

}]);