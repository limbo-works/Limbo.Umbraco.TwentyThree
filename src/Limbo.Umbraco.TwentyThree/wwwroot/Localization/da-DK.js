function meh(count, singular, plural) {
	count = parseInt(count);
	if (count === 1) return `${count} ${singular}`;
	if (count > 1 || count === 0) return `${count} ${plural}`;
	return plural;
}

export default {
	limboTwentyThree: {
		videos: (c) => meh(c, "video", "videoer"),
	}
}