
document.addEventListener('DOMContentLoaded', function () {
    var forms = document.querySelectorAll('.custom-form');

    forms.forEach(function (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();

            var submitButton = form.querySelector('button[type="submit"]');

            // Thực hiện kiểm tra form nếu bạn có trình xác thực
            if (form.checkValidity()) { // Sử dụng HTML5 form validation
                submitButton.setAttribute('data-kt-indicator', 'on');
                submitButton.disabled = true;

                axios.post(form.getAttribute('action'), new FormData(form)).then(function (response) {
                    if (response.data.message === "success") {
                        form.reset();

                        Swal.fire({
                            text: form.getAttribute("data-notify-success"),
                            icon: "success",
                            buttonsStyling: false,
                            confirmButtonText: "Ok!",
                            customClass: {
                                confirmButton: "btn btn-primary"
                            },
                            didClose: function () { // Hàm này được gọi khi hộp thoại đóng
                                location.reload(); // Tải lại trang
                            }
                        });

                        const redirectUrl = form.getAttribute('data-kt-redirect-url');
                        if (redirectUrl) {
                            location.href = redirectUrl;
                        }
                    } else {
                        console.log(JSON.stringify(response.data));
                        Swal.fire({
                            text: response.data.message,
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "Nhập lại",
                            customClass: {
                                confirmButton: "btn btn-primary"
                            }
                        });
                    }
                }).catch(function (error) {
                    Swal.fire({
                        text: "Có lỗi xảy ra, vui lòng thử lại!",
                        icon: "error",
                        buttonsStyling: false,
                        confirmButtonText: "Đóng",
                        customClass: {
                            confirmButton: "btn btn-primary"
                        }
                    });
                }).then(function () {
                    submitButton.removeAttribute('data-kt-indicator');
                    submitButton.disabled = false;
                });
            } else {
                Swal.fire({
                    text: "Có lỗi xảy ra, vui lòng thử lại!",
                    icon: "error",
                    buttonsStyling: false,
                    confirmButtonText: "Đóng",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                });
            }
        });
    });

    function formatCurrencyVND(number) {
        return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".") + ' VNĐ';
    }

    $(document).ready(function () {
        function formatCurrencyVND(number) {
            var absNumber = Math.abs(number); // Lấy giá trị tuyệt đối
            var formatted = absNumber.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });

            // Nếu số âm, thêm dấu trừ trước chuỗi đã định dạng
            return number < 0 ? `-${formatted}` : formatted;
        }

        $('.price').each(function () {
            var priceText = $(this).text();

            // Kiểm tra và xử lý giá trị âm
            var isNegative = priceText.trim().startsWith('-'); // Xác định nếu giá trị có dấu '-'
            var priceNumber = parseInt(priceText.replace(/[^\d-]/g, '')); // Bỏ ký tự không phải số, giữ dấu '-'

            if (isNaN(priceNumber)) {
                priceNumber = 0; // Gán mặc định nếu không phải số
            }

            // Định dạng lại giá trị thành VND
            var formattedPrice = formatCurrencyVND(priceNumber);

            // Cập nhật giá trị đã được định dạng
            $(this).text(formattedPrice);
        });




        /*$('.btn-show-password').on('click', function () {
            var $this = $(this);
            var userId = $this.data('id');  // Lấy ID người dùng từ thuộc tính data-id
            var $maskedPassword = $this.closest('td').find('.masked-password');

            if ($this.text() === 'XEM MK') {
                // Gửi yêu cầu AJAX để lấy mật khẩu
                $.ajax({
                    url: '/quantri/quanlynguoidung/GetPassword',  // URL đến action trong controller
                    type: 'GET',
                    data: { id: userId },  // ID của người dùng
                    success: function (response) {
                        // Hiển thị mật khẩu thật khi nhận được phản hồi từ server
                        $maskedPassword.text(response.password);
                        $this.text('ẨN MK');
                    }
                });
            } else {
                // Ẩn mật khẩu lại khi bấm nút "ẨN MK"
                $maskedPassword.text('********');
                $this.text('XEM MK');
            }
        });*/

    });

});