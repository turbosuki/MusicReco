using MusicReco.Domain;

namespace MusicReco;

internal interface INowPlayingProvider
{
    NowPlaying GetNowPlaying();
}
