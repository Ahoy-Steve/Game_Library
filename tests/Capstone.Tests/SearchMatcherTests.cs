using SharpQuest.Contracts;

namespace Capstone.Tests;

public class SearchMatcherTests
{
    private static readonly SearchMatcher Matcher = SearchMatcher.Create();

    private static bool Matches(string? text, string? query) => Matcher.Matches(text, query!);

    // R1: an empty or blank query matches every non-null text
    [Theory]
    [InlineData("Pikachu", "")]
    [InlineData("Pikachu", "   ")]
    [InlineData("Pikachu", "\t")]
    [InlineData("", "")]
    public void Blank_query_matches_any_non_null_text(string text, string query)
    {
        Assert.True(Matches(text, query), $"Query '{query}' is blank, so it should match '{text}' (R1).");
    }

    // R2: case is ignored
    [Theory]
    [InlineData("Pikachu", "PIKA")]
    [InlineData("Pikachu", "pika")]
    [InlineData("PIKACHU", "Pika")]
    [InlineData("pikachu", "pIkAcHu")]
    public void Case_is_ignored(string text, string query)
    {
        Assert.True(Matches(text, query), $"'{query}' should match '{text}' whatever the case (R2).");
    }

    // R3: diacritics are ignored, in the text AND in the query
    [Theory]
    [InlineData("Café", "cafe")]
    [InlineData("cafe", "Café")]
    [InlineData("șosea", "Sosea")]
    [InlineData("Sosea", "șosea")]
    [InlineData("Crème Brûlée", "creme brulee")]
    [InlineData("creme brulee", "CRÈME BRÛLÉE")]
    [InlineData("Cafe\u0301", "cafe")]   // the é written as e + a separate combining accent
    [InlineData("cafe", "Cafe\u0301")]
    public void Diacritics_are_ignored_in_both_directions(string text, string query)
    {
        Assert.True(Matches(text, query), $"'{query}' should match '{text}' once accents are ignored (R3).");
    }

    // R4: every word of the query must occur
    [Theory]
    [InlineData("Pikachu electric mouse", "pika mouse")]
    [InlineData("Pikachu electric mouse", "pika electric mouse")]
    public void Every_query_word_present_matches(string text, string query)
    {
        Assert.True(Matches(text, query), $"All words of '{query}' occur in '{text}' (R4).");
    }

    [Theory]
    [InlineData("Pikachu electric mouse", "pika zzz")]
    [InlineData("Pikachu electric mouse", "zzz pika")]
    [InlineData("fire lizard", "lizard water")]
    public void One_missing_query_word_means_no_match(string text, string query)
    {
        Assert.False(Matches(text, query), $"'{query}' has a word that isn't in '{text}', so it must not match (R4).");
    }

    // R5: word order doesn't matter
    [Theory]
    [InlineData("fire lizard", "lizard fire")]
    [InlineData("Charizard the fire lizard", "lizard fire")]
    [InlineData("one two three", "three one two")]
    public void Word_order_does_not_matter(string text, string query)
    {
        Assert.True(Matches(text, query), $"'{query}' should match '{text}' in any word order (R5).");
    }

    // R6: a word can be inside a longer word, at the start, the middle or the end
    [Theory]
    [InlineData("Pikachu", "pika")]
    [InlineData("Pikachu", "chu")]
    [InlineData("Pikachu", "kac")]
    public void A_word_can_match_inside_a_longer_word(string text, string query)
    {
        Assert.True(Matches(text, query), $"'{query}' occurs inside '{text}' (R6).");
    }

    // R7: a null text never matches. False, not an exception, even for a blank query.
    [Theory]
    [InlineData("pika")]
    [InlineData("")]
    [InlineData("   ")]
    public void Null_text_never_matches_and_does_not_throw(string query)
    {
        var ex = Record.Exception(() => Matches(null, query));

        Assert.True(ex is null, $"A null text with query '{query}' threw an exception ({ex?.Message}); it should just give false (R7).");
        Assert.False(Matches(null, query), $"A null text must never match, even with query '{query}' (R7).");
    }

    // R8: a null query throws ArgumentNullException
    [Fact]
    public void Null_query_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Matches("Pikachu", null));
    }
}