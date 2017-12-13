# coding:utf8
## Zero-truncated Poisson distributions for scipy
## formula from wikipedia
## https://en.wikipedia.org/wiki/Zero-truncated_Poisson_distribution

from numpy import log, floor
from numpy.random import uniform
from scipy.special import expm1
from scipy.stats import rv_discrete, poisson, nbinom

class ztpoisson_gen(rv_discrete):
    def _rvs(self, mu):
        return poisson.ppf(uniform(low=poisson.pmf(0, mu)), mu)
 
    def _pmf(self, k, mu):
        return -poisson.pmf(k, mu) / expm1(-mu)
 
    def _cdf(self, x, mu):
        k = floor(x)
        if k == 0:
            return 0.0
        else:
            return (poisson.cdf(k, mu) - poisson.pmf(0, mu)) / poisson.sf(0, mu) 

    def _ppf(self, q, mu):
        return poisson.ppf(poisson.sf(0, mu) * q + poisson.pmf(0, mu), mu)

    def _stats(self, mu):
        mean = mu * exp(mu) / expm1(mu)
        var = mean * (1.0 - mu / expm1(mu))
        g1 = None
        g2 = None
        return mean, var

ztpoisson = ztpoisson_gen(name="ztpoisson", longname='Zero-truncated Poisson')
