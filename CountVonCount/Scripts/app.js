"use strict";

$(document).ready(function () {

    $("#submitUrl").click(function () {
        console.log("form submitted");
        
        startLoading();

        $.post("api/crawl",
            $("#submitUrlForm").serialize(),
            function (response) {
                console.log(response);

                getWords();
            },
            "json"
        );
    });
});

function startLoading() {
    $("#wordCloud").hide();
    $("#wordsDiv").hide();
    $("#errorDiv").hide();
    $("#loadingDiv").show();
}

function endLoading() {
    $("#loadingDiv").hide();
    $("#wordCloud").show();
}

function showError() {
    $("#loadingDiv").hide();
    $("#wordCloud").hide();
    $("#wordsDiv").hide();
    $("#errorDiv").show();
}

function showHome() {
    $("#wordCloud").show();
    $("#wordsDiv").hide();
}

function showAdmin() {
    $("#wordCloud").hide();
    $("#wordsDiv").show();
}
function drawWordCloud(words) {
    $("#wordCloud").jQWCloud({
        words: words,
        minFont: 10,
        maxFont: 200,
        word_common_classes: "WordClass",
        word_mouseEnter: function () {
            $(this).css("text-decoration", "underline");
        },
        word_mouseOut: function () {
            $(this).css("text-decoration", "none");
        },
        word_click: function () {
            alert("You have selected:" + $(this).text());
        }
    });
}

function getWords() {
    $.getJSON("api/words")
        .done(function (data) {
            console.log(data);

            var words = Array();
            $("#wordsTable").empty();
            var $trHtml = "";

            $.each(data,
                function (i, item) {
                    words.push({ word: item.Word, weight: item.Count });

                    $trHtml += "<tr><td>" + (i+1) + "</td><td>" + item.Word + "</td><td>" + item.Count + "</td></tr>";
                });

            drawWordCloud(words);

            $("#wordsTable").append($trHtml);

            endLoading();
        })
        .catch(function (jqXHR, textStatus, err) {
            $("#errorDescription").html("Failed to retrieve words : " + err);

            showError();
        });
}