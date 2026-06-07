<!doctype html>
<html lang="${locale.currentLanguageTag!'pt-BR'}" class="gatewise-login-shell">
  <head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>${msg("gatewiseResetPasswordTitle")}</title>
    <link rel="icon" href="${url.resourcesCommonPath}/img/favicon.ico" />
    <link rel="stylesheet" href="${url.resourcesPath}/css/login.css?v=20260607-forgot-below-remember" />
    <link rel="stylesheet" href="${url.resourcesPath}/css/reset-password.css?v=20260607-reset-back-no-arrow-hover" />
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

      <section class="gw-auth" aria-label="${msg("gatewiseResetPasswordTitle")}">
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

        <div class="gw-card-wrapper gw-reset-wrapper">
          <a class="gw-back-button" href="${url.loginUrl}" aria-label="${msg("gatewiseBackToLogin")}">
            <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M10.8 19.2 3.6 12l7.2-7.2 1.4 1.4L7.4 11H20v2H7.4l4.8 4.8-1.4 1.4Z"/></svg>
          </a>

          <div class="gw-card gw-reset-card">
          <div class="gw-card-topline"></div>

          <header class="gw-heading">
            <span class="gw-kicker">${msg("gatewiseLoginAccess")}</span>
            <h2>${msg("gatewiseResetPasswordTitle")}</h2>
            <p>${msg("gatewiseResetPasswordSubtitle")}</p>
          </header>

          <#if message?has_content && (message.type != 'warning' || !isAppInitiatedAction??)>
            <div class="gw-alert gw-alert-${message.type}" role="alert">
              <span></span>
              <p>${kcSanitize(message.summary)?no_esc}</p>
            </div>
          </#if>

          <form id="kc-reset-password-form" class="gw-form" action="${url.loginAction}" method="post" onsubmit="var f=this,b=document.getElementById('kc-reset-password-submit');if(f.dataset.submitting==='true'){return true;}f.dataset.submitting='true';if(b){b.disabled=true;b.classList.add('is-loading');var t=b.querySelector('.gw-submit-text');if(t){t.textContent='${msg("gatewiseResetPasswordSending")}';}}setTimeout(function(){f.submit();},700);return false;">
            <label class="gw-field" for="username">
              <span>
                <#if !realm.loginWithEmailAllowed>
                  ${msg("username")}
                <#elseif !realm.registrationEmailAsUsername>
                  ${msg("usernameOrEmail")}
                <#else>
                  ${msg("email")}
                </#if>
              </span>
              <div class="gw-input">
                <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M20 4H4a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2V6a2 2 0 0 0-2-2Zm0 4-8 5L4 8V6l8 5 8-5Z"/></svg>
                <input id="username" name="username" type="text" value="${(auth.attemptedUsername!'')}" autocomplete="username" inputmode="email" autofocus placeholder="${msg("gatewiseUsernamePlaceholder")}" />
              </div>
            </label>

            <p class="gw-reset-hint">${msg("gatewiseResetPasswordInstruction")}</p>

            <button class="gw-submit" id="kc-reset-password-submit" type="submit">
              <span class="gw-submit-spinner" aria-hidden="true"></span>
              <span class="gw-submit-text">${msg("gatewiseResetPasswordSubmit")}</span>
              <svg class="gw-submit-icon" viewBox="0 0 24 24" aria-hidden="true"><path d="M13 5 11.6 6.4 16.2 11H4v2h12.2l-4.6 4.6L13 19l7-7Z"/></svg>
            </button>
          </form>
          </div>
        </div>
      </section>
    </main>
  </body>
</html>