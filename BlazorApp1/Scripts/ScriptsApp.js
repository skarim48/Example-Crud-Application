<head>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
    <script>
        function alerto(){
            alert('Done!');
    }

        $(document).ready(function(){
        // Get the button that opens the modal
        var btn = document.getElementsByClassName("editButton");
        var modal = document.getElementById("myModal");
        btn.onclick = function() {
            alert('eee');
        modal.style.display = "block";
        }
    });


    </script>
</head>