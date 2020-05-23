$(function () {
    var noteids = [];
    //i index, e element
    $("div[data-note-id]").each(function (i, e) {
        noteids.push($(e).data("note-id"));
    });
    $.ajax({
        method: "POST",
        url: "/Note/GetLiked",
        data: { ids: noteids }
    }).done(function (data) {
        console.log(data);
        if (data.result != null && data.result.length > 0) {
            for (var i = 0; i < data.result.length; i++) {
                var id = data.result[i];
                var likedNote = $("div[data-note-id=" + id + "]");
                var btn = likedNote.find("button[data-liked]");
                var span = btn.find("span.like-heart");

                btn.data("liked", true);
                span.removeClass("far fa-heart");
                span.addClass("fas fa-heart");
            }
        }
    }).fail(function () { });

    //sayfadaki button elementinin "data liked" nesnesinin click eventi
    $("button[data-liked").click(function () {
        var btn = $(this);
        var liked = btn.data("liked");
        var noteid = btn.data("note-id");
        var spanHeart = btn.find(".like-heart");
        var spanCount = btn.find(".like-count");

        $.ajax({
            method: "POST",
            url: "/Note/SetLikeState",
            data: { "noteid": noteid, "liked": !liked }
        }).done(function (data) {
            console.log(data);
            if (data.hasError) {
                alert(data.errorMessage);
            }
            else {
                liked = !liked;
                btn.data("liked", liked);
                spanCount.text(data.result);

                spanHeart.removeClass("far fa-heart");
                spanHeart.removeClass("fas fa-heart");

                if (liked) {
                    spanHeart.addClass("fas fa-heart")
                }
                else {
                    spanHeart.addClass("far fa-heart")
                }
            }
        }).fail(function () {
            alert("Sunucu ile bağlantı kurulamadı...")
        });
    });
});
