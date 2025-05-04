import asyncio
import logging
import random
import threading
from services import financial_times_service as financial_times, financial_atlas_service as financial_atlas
from models.financial_atlas_models import Company
from generators import macro_economic_article_generator, sector_article_generator, political_article_generator, company_article_generator, financial_report_generator, company_generator, market_data_generator
from constants import SECTORS

# Global control flag
stop_event = threading.Event()
worker_thread = None
logger = logging.getLogger("worker_app")

def __handle_sector_article_generation():
    """
    Gets the correct data from the api and generates a new article
    based on that data.
    """

    # Get articles from articles api
    try:
        sector = random.choice(SECTORS)
        logger.info("Starting article generation for sector: %s sector.", sector)
        sector_articles = financial_times.fetch_sector_articles(sector)
        macro_economic_articles = financial_times.fetch_macro_articles()
        political_articles = financial_times.fetch_political_articles()

        # Generate article via generator service with the inputs
        article = sector_article_generator.generate(sector_articles, 
                                                            macro_economic_articles, 
                                                            political_articles)
        financial_times.publish_article(article)
        logger.info("Article successfully published.")

    except Exception as e:
        raise Exception("An exception occurred while trying to generate a sector article.") from e


def __handle_company_generation():
    """
    Generates a new company based on a randomly generated ticker
    """
    try:
        logger.info("Starting company generation.")
        company = company_generator.generate()
        financial_atlas.create_company(company)
        market_data = market_data_generator.generate(company)
        financial_atlas.publish_market_data(market_data)
        logger.info("Successfully created new company with ticker %s", company.ticker)
    except Exception as e:
        raise Exception("An error occurred while trying to create a new company.") from e

async def __handle_company_article_generation():
    """
    Generates a new company article for a company randomly selected
    from a list of tickers
    """
    try:
        logger.info("Starting company article generation...")

        available_ticker = financial_atlas.fetch_tickers()
        selected_ticker = random.choice(available_ticker)

        # fetch company information
        company_profile : Company = financial_atlas.fetch_company(selected_ticker)

        # fetch information needed for to generate an article
        tasks = [
            financial_times.fetch_sector_articles(company_profile.industry),
            financial_times.fetch_macro_articles(),
            financial_times.fetch_company_articles(selected_ticker),
            financial_atlas.fetch_company_financials(selected_ticker),
            financial_times.fetch_political_articles()
            ]

        # wait for tasks to complete and unpack 
        sector_articles, macro_economic_articles, company_articles, financials, political_articles = await asyncio.gather(*tasks, return_exceptions=True)

        # Generate and publish new article
        article = company_article_generator.generate(
            sector_articles,
            macro_economic_articles,
            company_articles,
            financials, 
            political_articles
        )

        financial_times.publish_article(article)
        logger.info("Successfully published a new company article for company with ticker {ticker}", ticker=selected_ticker)
    except Exception as e:
        raise Exception("An exception occurred while trying to generate a company article.") from e
    

def __handle_political_article_generation():
    """
    Generates a new Political article.
    """
    try:
        logger.info("Started generating a new political article...")
        political_articles = financial_times.fetch_political_articles()
        article = political_article_generator.generate(political_articles)
        financial_times.publish_article(article)
        logger.info("Successfully published a new political article")
    except Exception as e:
        raise Exception("An exception occurred while trying to generate a political article.") from e

def __handle_macro_economic_generation():
    """
    Generates a new macro economic article
    """
    try:
        logger.info("Started generating a new macro economic article")

        macro_economic_articles = financial_times.fetch_macro_articles()
        political_articles = financial_times.fetch_political_articles()

        article = macro_economic_article_generator.generate(
            macro_economic_articles,
            political_articles
        )

        financial_times.publish_article(article)
        logger.info("Successfully published a new macro economic article.")
    except Exception as e:
        raise Exception("An exception occurred while trying to generate a macro economic article.") from e

def __handle_financial_report_generation():
    """
    
    """

def __handle_market_data_generation():
    """
    
    """

__generator_functions = [
#  __handle_sector_article_generation, 
#  __handle_company_article_generation, 
#  __handle_macro_economic_generation, 
#  __handle_political_article_generation,
 __handle_company_generation
]

def run():
    """
    Runs a background worker that generates data based on the current
    loop and so on the worker is in.
    """
    logger.info("Worker started.")
    while not stop_event.is_set():
        try:
            logger.info("Starting generation...")
            random.choice(__generator_functions)()
        except Exception as e:
            logger.exception(e)
    logger.info("Worker received stop signal and is exiting.")
