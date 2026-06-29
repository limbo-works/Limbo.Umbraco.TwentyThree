function meh(count, singular, plural) {
	count = parseInt(count);
	if (count === 1) return `${count} ${singular}`;
	if (count > 1 || count === 0) return `${count} ${plural}`;
	return plural;
}

export default {
	limboTwentyThree: {
		id: "ID",
		title: "Title",
		spot: "Spot",
        video: "Video",
		videos: (c) => meh(c, "video", "videos"),
	}
}