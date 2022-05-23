# uses vader lib (will be installed automatically via build commands) to identify sentiments in the body string
# return score result in the form of: {'neg': 0.0, 'neu': 0.323, 'pos': 0.677, 'compound': 0.6369}

from vaderSentiment.vaderSentiment import SentimentIntensityAnalyzer


def handler(context, event):
    body = event.body.decode('utf-8')
    context.logger.info('Analyzing sentence: ' + body)
    analyzer = SentimentIntensityAnalyzer()
    score = analyzer.polarity_scores(body)
    return context.Response(body=score,
                            headers={},
                            content_type='application/json',
                            status_code=200)  
