export function initializeSwiper(selector, direction) {


    const link = document.createElement('link');
    link.rel = 'stylesheet';
    link.href = 'https://cdn.jsdelivr.net/npm/swiper@11.1.5/swiper-bundle.min.css';
    document.head.appendChild(link);

    const script = document.createElement('script');
    script.src = 'https://cdn.jsdelivr.net/npm/swiper@11.1.5/swiper-bundle.min.js';
    script.onload = () => {
        var swiper = new Swiper(selector, {
            slidesPerView: 3,
            spaceBetween: 30,
            loop: true,
            centeredSlides: true,
            speed: 4000, // 动画速度
            autoplay: {
                delay: 0, // 自动滚动的时间间隔
                disableOnInteraction: false, // 用户操作后是否禁用自动滚动
                pauseOnMouseEnter: true,
                reverseDirection: direction??false
            }
        });
 
    };
    document.body.appendChild(script);

    console.log("initializeSwiper")
};

 