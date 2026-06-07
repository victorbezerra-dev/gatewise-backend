<!doctype html>
<html lang="${locale.currentLanguageTag!'pt-BR'}" class="gatewise-login-shell">
  <head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>${msg("gatewiseRegisterTitle")}</title>
    <link rel="icon" href="${url.resourcesCommonPath}/img/favicon.ico" />
    <link rel="stylesheet" href="${url.resourcesPath}/css/login.css?v=20260607-no-back-arrow-hover" />
    <link rel="stylesheet" href="${url.resourcesPath}/css/register.css?v=20260607-register-back-no-arrow-hover" />
  </head>
  <body class="gw-body">
    <main class="gw-page">
      <section class="gw-hero" aria-label="GateWise">
        <div class="gw-orb gw-orb-one"></div>
        <div class="gw-orb gw-orb-two"></div>

        <div class="gw-brand">
          <img
            src="${url.resourcesPath}/img/logo.png?v=20260605-logo-trimmed"
            alt="GateWise"
            style="display: block; width: 128px; height: 128px; max-width: 100%; object-fit: contain; filter: drop-shadow(0 22px 46px rgba(0, 0, 0, 0.46));"
          />
          <div class="gw-brand-copy">
            <span class="gw-kicker">${msg("gatewiseLoginEyebrow")}</span>
            <h1>${msg("gatewiseLoginHeroTitle")}</h1>
            <p>${msg("gatewiseLoginHeroSubtitle")}</p>
          </div>
        </div>

        <div class="gw-stats" aria-hidden="true">
          <div><strong>24/7</strong><span>${msg("gatewiseFeatureSecure")}</span></div>
          <div><strong>SSO</strong><span>${msg("gatewiseFeatureFast")}</span></div>
          <div><strong>ID</strong><span>${msg("gatewiseFeatureIntegrated")}</span></div>
        </div>
      </section>

      <section class="gw-auth" aria-label="${msg("gatewiseRegisterTitle")}">
        <div class="gw-mobile-brand" aria-hidden="true">
          <img
            src="${url.resourcesPath}/img/logo.png?v=20260605-logo-trimmed"
            alt="GateWise"
            style="display: block; width: 140px; height: auto; max-width: 62vw; margin: 0 auto; object-fit: contain; filter: drop-shadow(0 18px 34px rgba(0, 0, 0, 0.42));"
          />
          <div class="gw-mobile-brand-copy">
            <span class="gw-kicker">${msg("gatewiseLoginEyebrow")}</span>
            <h1>${msg("gatewiseLoginHeroTitle")}</h1>
            <p>${msg("gatewiseLoginHeroSubtitle")}</p>
          </div>
        </div>

        <div class="gw-card-wrapper gw-register-wrapper">
          <a class="gw-back-button" href="${url.loginUrl}" aria-label="${msg("gatewiseBackToLogin")}">
            <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M10.8 19.2 3.6 12l7.2-7.2 1.4 1.4L7.4 11H20v2H7.4l4.8 4.8-1.4 1.4Z"/></svg>
            <span>${msg("gatewiseBackToLogin")}</span>
          </a>

          <div class="gw-card gw-register-card">
          <div class="gw-card-topline"></div>

          <header class="gw-heading">
            <span class="gw-kicker">${msg("gatewiseLoginAccess")}</span>
            <h2>${msg("gatewiseRegisterTitle")}</h2>
            <p>${msg("gatewiseRegisterSubtitle")}</p>
          </header>

          <#if message?has_content && (message.type != 'warning' || !isAppInitiatedAction??)>
            <div class="gw-alert gw-alert-${message.type}" role="alert">
              <span></span>
              <p>${kcSanitize(message.summary)?no_esc}</p>
            </div>
          </#if>

          <form id="kc-register-form" class="gw-form" action="${url.registrationAction}" method="post">
            <label class="gw-field" for="firstName">
              <span>${msg("firstName")}</span>
              <div class="gw-input">
                <input id="firstName" name="firstName" type="text" value="${((register.formData.firstName)!'')}" autocomplete="given-name" />
              </div>
            </label>

            <label class="gw-field" for="lastName">
              <span>${msg("lastName")}</span>
              <div class="gw-input">
                <input id="lastName" name="lastName" type="text" value="${((register.formData.lastName)!'')}" autocomplete="family-name" />
              </div>
            </label>

            <label class="gw-field" for="email">
              <span>${msg("email")}</span>
              <div class="gw-input">
                <input id="email" name="email" type="email" value="${((register.formData.email)!'')}" autocomplete="email" placeholder="${msg("gatewiseEmailPlaceholder")}" />
              </div>
            </label>

            <#if !realm.registrationEmailAsUsername>
              <label class="gw-field" for="username">
                <span>${msg("username")}</span>
                <div class="gw-input">
                  <input id="username" name="username" type="text" value="${((register.formData.username)!'')}" autocomplete="username" placeholder="${msg("gatewiseUsernamePlaceholder")}" />
                </div>
              </label>
            </#if>

            <#if passwordRequired??>
              <label class="gw-field" for="password">
                <span>${msg("password")}</span>
                <div class="gw-input">
                  <input id="password" name="password" type="password" autocomplete="new-password" placeholder="${msg("gatewisePasswordPlaceholder")}" />
                  <button class="gw-toggle" type="button" aria-label="${msg("gatewiseShowPassword")}" onclick="const input=document.getElementById('password'); input.type=input.type==='password'?'text':'password'; this.classList.toggle('active');">
                    <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M12 5c5.5 0 9.5 5 10 7-.5 2-4.5 7-10 7S2.5 14 2 12c.5-2 4.5-7 10-7Zm0 3a4 4 0 1 0 0 8 4 4 0 0 0 0-8Zm0 2a2 2 0 1 1 0 4 2 2 0 0 1 0-4Z"/></svg>
                  </button>
                </div>
              </label>

              <label class="gw-field" for="password-confirm">
                <span>${msg("passwordConfirm")}</span>
                <div class="gw-input">
                  <input id="password-confirm" name="password-confirm" type="password" autocomplete="new-password" placeholder="${msg("gatewisePasswordConfirmPlaceholder")}" />
                  <button class="gw-toggle" type="button" aria-label="${msg("gatewiseShowPassword")}" onclick="const input=document.getElementById('password-confirm'); input.type=input.type==='password'?'text':'password'; this.classList.toggle('active');">
                    <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M12 5c5.5 0 9.5 5 10 7-.5 2-4.5 7-10 7S2.5 14 2 12c.5-2 4.5-7 10-7Zm0 3a4 4 0 1 0 0 8 4 4 0 0 0 0-8Zm0 2a2 2 0 1 1 0 4 2 2 0 0 1 0-4Z"/></svg>
                  </button>
                </div>
              </label>
            </#if>

            <#if recaptchaRequired??>
              <div class="g-recaptcha" data-size="compact" data-sitekey="${recaptchaSiteKey}"></div>
            </#if>

            <button class="gw-submit" type="submit">
              <span class="gw-submit-text">${msg("doRegister")}</span>
              <svg class="gw-submit-icon" viewBox="0 0 24 24" aria-hidden="true"><path d="M13 5 11.6 6.4 16.2 11H4v2h12.2l-4.6 4.6L13 19l7-7Z"/></svg>
            </button>
          </form>

          <p class="gw-register">${msg("alreadyAccount")} <a href="${url.loginUrl}">${msg("doLogIn")}</a></p>
          </div>
        </div>
      </section>
    </main>
  </body>
</html>