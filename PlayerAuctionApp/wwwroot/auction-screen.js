window.auctionScreen = {
    toggle: async () => {
        if (!document.fullscreenElement) {
            await document.documentElement.requestFullscreen();
            return true;
        }

        await document.exitFullscreen();
        return false;
    }
};
